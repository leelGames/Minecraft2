using UnityEngine;

public class VoxelEntity : AChunk {
	World world;
	Rigidbody rb;
	Vector3Int pos;
	Vector3Int width;
	Vector3 v;
	bool moving;
	Vector3Int newposition;
	Quaternion newrotation;


	public void Init(World w, Vector3Int pos1, Vector3Int pos2, int size) {
		Init(pos2.x - pos1.x, pos2.y - pos1.y, pos2.z - pos1.z);
		render = new VoxelRenderer(this);
		meshCollider.convex = true;
		world = w;
		pos = pos1;
		width = pos2 - pos1;

		rb = gameObject.AddComponent<Rigidbody>();
		rb.mass = size * 10;
		rb.interpolation = RigidbodyInterpolation.Interpolate;
	
		gameObject.transform.SetParent(world.transform);
		gameObject.transform.position = pos;
		gameObject.name = "VoxelEntity ";
		moving = true;
	}

	public static VoxelEntity Create(World world, Vector3Int[] voxels) {
		Vector3Int min = voxels[0];
		Vector3Int max = voxels[0];

		for (int i = 0; i < voxels.Length; i++) {
			//Chunk c = world.GetChunkP(voxels[i]);
			//c.SetFlag(voxels[i] - c.Position, false);

			if (voxels[i].x < min.x) min.x = voxels[i].x;
			else if (voxels[i].x > max.x) max.x = voxels[i].x;
			if (voxels[i].y < min.y) min.y = voxels[i].y;
			else if (voxels[i].y > max.y) max.y = voxels[i].y;
			if (voxels[i].z < min.z) min.z = voxels[i].z;
			else if (voxels[i].z > max.z) max.z = voxels[i].z;
		}
		VoxelEntity ve = new GameObject().AddComponent<VoxelEntity>();
		ve.Init(world, min, max + Vector3Int.one, voxels.Length);
		ve.Active = false;

		VoxelData v;
		for (int i = 0; i < voxels.Length; i++) {
			v = world.GetVoxelData(voxels[i]);
			ve.SetVoxel(voxels[i] - min, v.block.id, v.mainAtr);
		}

		UpdateEvent e = new(Vector2Int.zero, 0, false, true);
		ve.UpdateChunk(e);
		ve.render.DrawMesh(e);
		return ve;
	}

	void FixedUpdate() {
		if (moving) {
			if (rb.velocity.magnitude < 0.1 && rb.velocity.magnitude < v.magnitude && rb.angularVelocity.magnitude < 0.01) {
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				meshCollider.enabled = false;
				rb.isKinematic = true;
				newposition = Vector3Int.RoundToInt(transform.position + (width / 2)) - (width / 2);
				newrotation = Quaternion.Euler(GetRotation(transform.eulerAngles));
				/*Vector3 center = rb.centerOfMass;
				newrotation = Quaternion.Euler(GetRotation(transform.eulerAngles));
				Vector3 newcenter = newrotation * center;
				newposition = Vector3Int.RoundToInt(transform.position - center + newcenter);*/
				//newrotation = Quaternion.LookRotation(Vector3Int.RoundToInt(transform.forward), Vector3Int.RoundToInt(transform.up));
				moving = false;
			}
		}
		else {
			if (Mathf.Abs(transform.position.magnitude - newposition.magnitude) > 0.01f || Quaternion.Angle(transform.rotation, newrotation) > 1f) {
				transform.SetPositionAndRotation(Vector3.Lerp(transform.position, newposition, Time.fixedDeltaTime * 2f), Quaternion.Lerp(transform.rotation, newrotation, Time.fixedDeltaTime * 2f));
			} else {
				transform.SetPositionAndRotation(newposition, newrotation);
				Place();
			}
		}	
		v = rb.velocity;
	}

	void Place() {
		Vector3Int pos;
		for (int x = 0; x < width.x; x++) {
			for (int z = 0; z < width.z; z++) {
				for (int y = 0; y < width.y; y++) {
					if (BD.blocks[GetVoxel(x, y, z)].type != BType.Air) {
						pos = newposition + Vector3Int.RoundToInt(VoxelRenderer.RotateVert(new Vector3(x, y, z), newrotation));
						if (world.GetVoxel(pos).type != BType.Terrain) {
							world.SetVoxel(pos, GetVoxel(x, y, z), BD.blocks[GetVoxel(x, y, z)].rotMode != RMode.None ? RotateDir(GetVoxelAtr(x, y, z), newrotation) : 0);
							world.UpdateBlockFast(pos);
							//world.threads.chunksToUpdate.AddBuffer(new UpdateEvent(world.GetChunkCoord(pos), 0, false, true));
						}
					}
				}
			}
		}
		Destroy(gameObject, 0.05f);
	}

	public void Fill() {
		VoxelData block;
		for (int x = 0; x < width.x; x++) {
			for (int z = 0; z < width.z; z++) {
				for (int y = 0; y < width.y; y++) {
					block = world.GetVoxelData(pos + new Vector3Int(x, y, z));
					SetVoxel(new Vector3Int(x, y, z), block.block.id, block.mainAtr);
				}
			}
		}
	}

	public override void UpdateChunk(UpdateEvent e) {
		for (int y = 0; y < width.y; y++) {
			for (int x = 0; x < width.x; x++) {
				for (int z = 0; z < width.z; z++) {
					render.UpdateMesh(e, new Vector3Int(x, y, z));
				}
			}
		}
	}

	public override Block CheckBlock(Vector3Int pos) {
		if (!IsVoxelInChunk(pos)) {
			return BD.blocks[0];
		}
		return GetVoxel(pos);
	}

	public override int CheckBlockAtr(Vector3Int pos) {
		if (!IsVoxelInChunk(pos)) {
			return 0;
		}
		return GetVoxelAtr(pos);
	}

	public override Bounds CheckBounds(Vector3Int pos) {
		if (!IsVoxelInChunk(pos)) {
			return new Bounds();
		}
		return GetBounds(pos);
	}

	Vector3Int GetRotation(Vector3 rot) {
		return new Vector3Int(Mathf.RoundToInt(rot.x / 90) * 90, Mathf.RoundToInt(rot.y / 90) * 90, Mathf.RoundToInt(rot.z / 90) * 90);
	}

	int RotateDir(int dir, Quaternion rot) {
		
		Quaternion rotation = Quaternion.Euler(VD.dirToRot2[dir]) * rot;
		//fix scheis rundungsfeler
		rotation = Quaternion.Euler(Vector3Int.RoundToInt(rotation.eulerAngles));
		
		for (int i = 0; i < VD.dirToRot2.Length; i++) {
		
			if (rotation == Quaternion.Euler(VD.dirToRot2[i])) return i;
		}
		Debug.Log("errör");
		return 0;
		/*int[] yt = new int[] { 0, 2, 1, 3 };
		int[,] xzt = new int[,] {
			{ 0, 4, 1, 5 },
			{ 2, 0, 0, 0 },
			{ 0, 0, 0, 0 },
			{ 3, 0, 0, 0 }
		};
		//Debug.Log(rotation + " " + (xzt[rotation.z / 90, rotation.x / 90] * 4 + yt[rotation.y / 90]));
		return xzt[rotation.z / 90, rotation.x / 90] * 4 + yt[rotation.y / 90];*/
		
	}
}
