using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VoxelRenderer {
	readonly AChunk chunk;
	public Mesh mesh;
	public Mesh lodmesh;
	public Mesh collider;

	readonly List<Vector3> vertices;
	readonly List<int> terrainTris;
	readonly List<int> transparentTris;
	readonly List<int> uvTris;
	readonly List<int> waterTris;
	readonly List<Vector3> solidverts;
	readonly List<int> solidTris;
	readonly List<Vector2> uvs;
	readonly List<Vector2> uvs2;

	Block thisBlock;
	int thisAtr;
	byte[] thisData;

	protected int vertCount;
	protected int solidvertCount;
	
	public VoxelRenderer(AChunk chunk) {
		this.chunk = chunk;
		vertices = new();
		uvs = new();
		terrainTris = new();
		transparentTris = new();
		uvTris = new();
		waterTris = new();
		solidverts = new();
		solidTris = new();
		uvs2 = new();
	}

	public void UpdateMesh(UpdateEvent e, Vector3Int pos) {
		thisBlock = chunk.GetBlock(pos);

		if (e.lod == 0) {
			switch (thisBlock.type) {
				case BType.Air: return;
				case BType.Liquid:
					thisAtr = chunk.GetVoxelAtr(pos);
					GetMeshLiquid(pos, e); return;
				case BType.Terrain: 
					GetMeshTerrain(pos, e); return;
				case BType.Rounded: 
					GetMeshRounded(pos, e); return;
				case BType.Custom: 
					thisAtr = chunk.GetVoxelAtr(pos);
					GetMeshCustom(pos, e); return;
				case BType.Voxel:
					thisAtr = chunk.GetVoxelAtr(pos);
					GetMeshVoxel(pos, e); return;
				case BType.Slope: 
					thisAtr = chunk.GetVoxelAtr(pos);
					thisData = chunk.GetData(pos);
					GetMeshSlope(pos, e); return;
				case BType.Combination: 
					thisData = chunk.GetData(pos);
					GetMeshCombination(pos, e); return;
			}
		} else {
			switch (thisBlock.type) {
				case BType.Air: return;
				case BType.Liquid: case BType.Custom: case BType.Combination:
					thisAtr = chunk.GetVoxelAtr(pos);
					GetMeshBounds(pos, e); return;
				case BType.Terrain: 
					GetMeshTerrain(pos, e); return;
				case BType.Rounded: 
					GetMeshRounded(pos, e); return;
				case BType.Voxel:
					thisAtr = chunk.GetVoxelAtr(pos);
					GetMeshVoxel(pos, e); return;
				case BType.Slope:
					thisAtr = chunk.GetVoxelAtr(pos);
					thisData = chunk.GetData(pos);
					GetMeshSlope(pos, e); return;
			}
		}
	}

	public void DrawMesh(UpdateEvent e) {
		if (e.lod == 0) {
			chunk.meshRenderer.materials = new Material[] { Main.materials[0], Main.materials[1], Main.materials[2], Main.materials[3] };
			mesh = new();
			if (vertCount > 65535) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			mesh.vertices = vertices.ToArray();
			mesh.subMeshCount = 4;
			mesh.SetTriangles(terrainTris.ToArray(), 0);
			mesh.SetTriangles(transparentTris.ToArray(), 1);
			mesh.SetTriangles(uvTris.ToArray(), 2);
			mesh.SetTriangles(waterTris.ToArray(), 3);
			mesh.uv = uvs.ToArray();
			mesh.uv2 = uvs2.ToArray();
			mesh.RecalculateNormals();
			chunk.meshFilter.mesh = mesh;
		}
		else {
			chunk.meshRenderer.materials = new Material[] { Main.materials[4], Main.materials[5]};
			lodmesh = new();
			if (vertCount > 65535) lodmesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			lodmesh.vertices = vertices.ToArray();
			lodmesh.uv = uvs.ToArray();
			lodmesh.subMeshCount = 2;
			lodmesh.SetTriangles(terrainTris.ToArray(), 0);
			lodmesh.SetTriangles(transparentTris.ToArray(), 1);

			lodmesh.RecalculateNormals();
			chunk.meshFilter.mesh = lodmesh;
			//chunk.meshCollider.sharedMesh = null;
		}
		if (e.collider) {
			collider = new();
			if (solidvertCount > 65535) collider.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			collider.vertices = solidverts.ToArray();
			collider.triangles = solidTris.ToArray();
			chunk.meshCollider.sharedMesh = collider;
		}
		vertices.Clear();
		terrainTris.Clear();
		transparentTris.Clear();
		uvTris.Clear();
		solidverts.Clear();
		solidTris.Clear();
		waterTris.Clear();
		uvs.Clear();
		uvs2.Clear();
		vertCount = 0;
		solidvertCount = 0;
	}

	void AddVert(Vector3 vert, Vector2 uv, UpdateEvent e, Block block) {
		if (e.lod == 0) {
			vertices.Add(vert);
			uvs.Add(new Vector2(block.textureID, block.textureScale));

			if (block.shaderType < SMode.Triplanar) uvs2.Add(uv);
			else uvs2.Add(Vector2.zero);
		} else {
			vertices.Add(vert);
			uvs.Add(new Vector2(0, block.textureID / 64f));
		}
		vertCount++;

		if (e.collider && block.isSolid) {
			solidverts.Add(vert);
			solidvertCount++;
		}
	}

	void AddTri(int tri, int offset, UpdateEvent e, Block block) {
		int value = tri + vertCount - offset;
		if (e.lod == 0) {
			if (block.shaderType == SMode.Triplanar) terrainTris.Add(value);
			else if (block.shaderType == SMode.UVCutoff) uvTris.Add(value);
			else if (block.shaderType == SMode.UVAlpha) transparentTris.Add(value);
			else if (block.shaderType == SMode.Water) waterTris.Add(value);
		} else {
			if (block.shaderType == SMode.Water) transparentTris.Add(value);
			else terrainTris.Add(value);
		}

		if (e.collider && block.isSolid) {
			solidTris.Add(tri + solidvertCount - offset);
		}
	}

	public int GetMeshIndex(Vector3Int pos) {
		Block thisBlock = chunk.GetBlock(pos);
		int thisAtr = chunk.GetVoxelAtr(pos);
		//Fügt gespeicherte Ecken und Dreiecke hinzu
		int meshIndex = 0;
		if (thisBlock.connectMode != CMode.None) {
			if (thisBlock.connectMode == CMode.Random) {
				meshIndex = 0;// Worldgen.Range(pos, 0, BD.meshcount[thisBlock.meshID], 5);
			}
			else if (thisBlock.connectMode == CMode.Grid) {
				//ermittelt id anhand benachbarter grid Blöcke
				for (int i = 0; i < 6; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).id == thisBlock.id) {
						meshIndex |= 1 << i;
					}
				}
				if (meshIndex == 0b111111) return 0;
			} else if (thisBlock.connectMode == CMode.Pipe) {
				//ermittelt id anhand benachbarter pipe Blöcke und richtung
				thisAtr /= 8;
				for (int i = 0; i < 6; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).id == thisBlock.id && (thisAtr == i / 2 || chunk.CheckBlockAtr(pos + VD.dirs[i]) / 8 == i / 2)) {
						meshIndex |= 1 << i;
					}
				}
				meshIndex += thisAtr * 64;
			} else {//Horizontal und vertical
				int offset;
				Vector3Int next;
				if (thisBlock.connectMode == CMode.Horizontal) offset = 2;
				else offset = 0; //Vertical
				for (int i = 0; i < 4; i++) {
					next = pos + Vector3Int.RoundToInt(VD.dirToRot3[thisBlock.slabType == 0 ? thisAtr : thisAtr / 3] * ((Vector3)VD.dirs[i + offset]));
					if (chunk.CheckBlock(next).id == thisBlock.id && chunk.CheckBlockAtr(next) == thisAtr) {
						meshIndex |= 1 << i;
					}
				}
				if (meshIndex == 15) return 0;
			}
		}
		return meshIndex;
	}

	protected void GetMeshCustom(Vector3Int pos, UpdateEvent e) {
		//Bounds b = chunk.GetBounds(pos);

		//umschlossene Blöcke werden nicht geupdated
		int check = 0;
		for (int i = 0; i < 6; i++) {
			if (thisBlock.isTransparent){
				if (chunk.CheckBlock(pos + VD.dirs[i]).type < BType.Terrain) check++;
			}
			else {
				if (chunk.CheckBlock(pos + VD.dirs[i]).isTransparent) check++;
			}
		}
		if (check == 0) return;
		Mesh2 mesh = Main.meshTable[thisBlock.meshID][GetMeshIndex(pos)];

		if (thisBlock.slabType == 0) {
			if (thisBlock.rotMode == RMode.None || thisBlock.connectMode >= CMode.Grid ) {
				for (int i = 0; i < mesh.vertices.Length; i++) {
					AddVert(mesh.vertices[i] + pos, mesh.uv[i], e, thisBlock);
				}
			} else {
				for (int i = 0; i < mesh.vertices.Length; i++) {
					AddVert(RotateVert(mesh.vertices[i], thisAtr) + pos /*- new Vector3(0, b.min.y, 0)*/, mesh.uv[i], e, thisBlock);
				}
			}
		}
		else {
			for (int i = 0; i < mesh.vertices.Length; i++) {
				AddVert(RotateVert(mesh.vertices[i] + new Vector3(0f, 0f, thisAtr % 3 / 3f), thisAtr / 3) + pos, mesh.uv[i], e, thisBlock);
			}
		}
		for (int j = 0; j < mesh.triangles.Length; j++) {
			AddTri(mesh.triangles[j], mesh.vertices.Length, e, thisBlock);
		}
		//vertCount += Main.meshTable[thisBlock.meshID][meshIndex].vertices.Length;
	}

	//Ermittelt genau, ob Voxelfaces gerendert werden müssen
	public static bool DrawVoxelFace(Bounds thisBounds, Bounds nextBounds, int dir) {
		return dir switch {
			0 => (thisBounds.min.y != 0f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			1 => (thisBounds.max.y != 1f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			2 => (thisBounds.min.x != 0f) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			3 => (thisBounds.max.x != 1f) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			4 => (thisBounds.min.z != 0f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			5 => (thisBounds.max.z != 1f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			_ => true
			//Geht noch nich ganz
			/*0 => (thisBounds.min.y + 1 > nextBounds.max.y) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			1 => (thisBounds.max.y - 1 < nextBounds.min.y) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			2 => (thisBounds.min.x + 1 > nextBounds.max.x) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			3 => (thisBounds.max.x - 1 < nextBounds.min.x) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			4 => (thisBounds.min.z + 1 > nextBounds.max.z) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			5 => (thisBounds.max.z - 1 < nextBounds.min.z) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			_ => true*/
		};
	}

	void GetMeshVoxel(Vector3Int pos, UpdateEvent e) {
		Bounds b = VD.slabBounds[thisAtr];
		
		for (int i = 0; i < 6; i++) {
			//prüft ob Fläche sichtbar ist				
			if (chunk.CheckBlock(pos + VD.dirs[i]).type != BType.Voxel || DrawVoxelFace(b, chunk.CheckBounds(pos + VD.dirs[i]), i)) { 
				//definert Ecken der Fläche
				for (int j = 0; j < 4; j++) {
					AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.voxelTris[i, j]], b.size), VD.voxelUvs[j], e, thisBlock);
				}
				//Definert 2 Dreiecke für quadratische Fläche
				for (int k = 0; k < 6; k++) {
					AddTri(VD.vertexTris[k], 4, e, thisBlock);
				}
				//vertCount += 4;
			}
		}
	}

	void GetMeshSlope(Vector3Int pos, UpdateEvent e) {
		Bounds b = VD.slabBounds[thisBlock.slabType == 0 ? 0 : thisData[0]];

		int dir;
		if (thisBlock.slabType != 0) dir = thisData[1] / 2;
		else dir = thisAtr / 2;

		//Fügt gespeicherte Ecken und Dreiecke hinzu
		for (int i = 0; i < VD.slopeTris.Length; i++) {
			AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.vertRots[dir, VD.slopeTris[i]]], b.size), VD.voxelUvs[VD.slopeUVs[i]], e, thisBlock);
			AddTri(0, 1, e, thisBlock);
		}
	}

	void GetMeshTerrain(Vector3Int pos, UpdateEvent e) {
		Block check;
		byte marchIndex = 0;

		for (int i = 0; i < 26; i++) {
			//Prüft welcher benachbarte Block leer ist um ID in der Tabelle zu ermitteln
			check = chunk.CheckBlock(pos + VD.dirs[i]);
			if (check.type != BType.Terrain) { // || (check.type == 3 && check.variant == 0))) { // || (VD.tCheck[i].y == -1 && CheckBlock(pos + VD.tCheck[i]).type != 2)) {
				marchIndex |= VD.tBytes[i];
			}
			//Ein schwebender Block wird gelöscht
			if (marchIndex == 255) {
				for (int j = 2; j < 6; j++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).type == BType.Liquid) {
						chunk.SetVoxel(pos, 29, 12);
						return;
					}
				} 
				chunk.SetVoxel(pos, 0);
				return;
			}
		}
		if (marchIndex == 0) return;
		//Terrain Flach?
		for (int i = 0; i < 6; i++) {
			if (marchIndex == VD.tBytes[i]) { chunk.SetVoxel(pos, thisBlock.id, 1); break; }
		}

		int edgeIndex = 0;
		int num;

		//Berechnet Position der Ecken aus tabelle
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 3; j++) {
				num = VD.tTriangles[marchIndex, edgeIndex];
				if (num == -1) return;

				AddVert((pos + VD.voxelVerts[VD.tEdges[num, 0]] + (pos + VD.voxelVerts[VD.tEdges[num, 1]]) + Vector3.up) / 2f, Vector2.zero, e, thisBlock);
				AddTri(0, 1, e, thisBlock);
				edgeIndex++;
			}
		}
	}

	void GetMeshRounded(Vector3Int pos, UpdateEvent e) {
		Block check;
		byte marchIndex;

		//Optimieren!!!
		Vector3Int newpos;
		for (int k = 0; k < 26; k++) {
			if (chunk.CheckBlock(pos + VD.dirs[k]).type < BType.Rounded) {
				newpos = pos + VD.dirs[k];
				marchIndex = 0;
				for (int i = 0; i < 26; i++) {
					//Prüft welcher benachbarte Block leer ist um ID in der Tabelle zu ermitteln
					check = chunk.CheckBlock(newpos + VD.dirs[i]);
					if (check.type == BType.Rounded) {
						marchIndex |= VD.tBytes[i];
					}
					if (marchIndex == 255) {
					//	chunk.SetVoxel(newpos, thisBlock.id);
					}
				}
				if (marchIndex == 0) return;

				//int edgeIndex = 0;
				int num;
				//Berechnet Position der Ecken aus tabelle
				for (int edgeIndex = 15; edgeIndex >= 0; edgeIndex--) {

					num = VD.tTriangles[marchIndex, edgeIndex];
					if (num != -1) {
						AddVert(newpos + (Vector3)(VD.voxelVerts[VD.tEdges[num, 0]] + VD.voxelVerts[VD.tEdges[num, 1]]) / 2f, Vector2.zero, e, thisBlock);
						AddTri(0, 1, e, thisBlock);
					}
				}
			}
		}
	}

	void GetMeshLiquid(Vector3Int pos, UpdateEvent e) {
		int height;

		for (int i = 0; i < 6; i++) {
			//definert Ecken der Fläche							//optimeren und waterlogging implementieren
			if (chunk.CheckBlock(pos + VD.dirs[i]).type == BType.Air  || i == 1 && chunk.CheckBlock(pos + VD.dirs[i]).type != BType.Liquid) {
				for (int j = 0; j < 4; j++) {
					Vector3 vert = VD.voxelVerts[VD.voxelTris[i, j]];

					//Höhe
					if (vert.y != 0) {
						//Static
						height = thisAtr;
						if (thisBlock.slabType == 1) {
							vert -= new Vector3(0, 1 - (height / 16f), 0f);
						}
						else {
							//Dynamic
							for (int k = 0; k < 8; k++) {
								Vector3Int dir = pos + new Vector3Int(VD.chunkChecks[k].x, 1, VD.chunkChecks[k].y);

								if (chunk.CheckBlock(dir).type == BType.Liquid && (VD.waterHeight[k, VD.voxelTris[i, j]] == 1)) {
									height = thisBlock.meshID + chunk.CheckBlockAtr(dir);
								}
								else {
									dir = pos + new Vector3Int(VD.chunkChecks[k].x, 0, VD.chunkChecks[k].y);
									if (chunk.CheckBlock(dir).type == BType.Liquid && (chunk.CheckBlockAtr(dir) * VD.waterHeight[k, VD.voxelTris[i, j]] > height)) {
										height = chunk.CheckBlockAtr(dir);
									}
								}
							}
							vert += (thisBlock.meshID - height) / (float) thisBlock.meshID * Vector3.down;
						}
					}

					//Mesh füllt benachbarte Blöcke aus
					for (int k = 2; k < 6; k++) {
						if (chunk.CheckBlock(pos + VD.dirs[k] * -1).type > BType.Liquid) {
							vert += VD.voxelFaces[k, VD.voxelTris[i, j]] * (Vector3)VD.dirs[k];
						}
					}
					AddVert(pos + vert, Vector2.zero, e, thisBlock);
				}

				//Definert 2 Dreiecke für quadratische Fläche
				for (int k = 0; k < 6; k++) {
					AddTri(VD.vertexTris[k], 4, e, thisBlock);
				}
				//vertCount += 4;
			}
		}
	}

	void GetMeshCombination(Vector3Int pos, UpdateEvent e) {
		byte[] data = thisData;
		int offset = 0;
		int count = 2;//thisBlock.slabType;

		for (int j = 0; j < count; j++) {
			thisBlock = BD.blocks[data[offset]];
			
			if (thisBlock.dataSize.Length == 1) thisAtr = data[offset + 1];
			else if (thisBlock.dataSize.Length > 1) {
				thisData = new byte[thisBlock.dataSize.Length];
				for (int i = 0; i < thisBlock.dataSize.Length; i++) {
					thisData[i] = data[offset + i + 1];
				}
			}
			switch (thisBlock.type) {
				case BType.Air: break;
				case BType.Liquid: GetMeshLiquid(pos, e); break;
				case BType.Terrain: GetMeshTerrain(pos, e); break;
				case BType.Rounded: GetMeshRounded(pos, e); break;
				case BType.Custom: GetMeshCustom(pos, e); break;
				case BType.Voxel: GetMeshVoxel(pos, e); break;
				case BType.Slope: GetMeshSlope(pos, e); break;
			}
			offset += thisBlock.dataSize.Length + 1;
		}
	}

	void GetMeshBounds(Vector3Int pos, UpdateEvent e) {
		Bounds b = chunk.GetBounds(pos);
		Block nextBlock;

		for (int i = 0; i < 6; i++) {
			//prüft ob Fläche sichtbar ist			
			nextBlock = chunk.CheckBlock(pos + VD.dirs[i]);
			if (nextBlock.type < BType.Terrain && thisBlock.type != BType.Liquid || nextBlock.type == BType.Air) {
				//definert Ecken der Fläche
				for (int j = 0; j < 4; j++) {
					AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.voxelTris[i, j]], b.size), VD.voxelUvs[j], e, thisBlock);
				}
				//Definert 2 Dreiecke für quadratische Fläche
				for (int k = 0; k < 6; k++) {
					AddTri(VD.vertexTris[k], 4, e, thisBlock);
				}
			}
		}
	}

	//Für implementierung von rotierung
	public static Vector3 RotateVert(Vector3 vert, int dir) {
		Vector3 center = new(0.5f, 0.5f, 0.5f);
		return VD.dirToRot3[dir] * (vert - center) + center;
	}
	
	public static Vector3 RotateVert(Vector3 vert, Quaternion rotation) {
		Vector3 center = new(0.5f, 0.5f, 0.5f);
		return rotation * (vert + center) - center;
	}

	//ToDo VoexelEntity rotations

	public static int rotateDirBlock(int dir, Quaternion rotation) {
		
		rotation = (VD.dirToRot3[dir] * rotation).normalized;

		/*rotation.ToAngleAxis(out float angle, out Vector3 axis);
		Debug.Log(axis.ToString("F10"));
		Debug.Log(angle.ToString("F10"));*/
		float dot;
		float max = 0;
		int maxIndex = 0;
		for (int i = 0; i < VD.dirToRot3.Length; i++) {
			dot = Quaternion.Dot(rotation, VD.dirToRot3[i]);
			if (dot > max) {max = dot; maxIndex = i;}
		}
		//Debug.LogWarning("Place Rotation not found");
		
		return maxIndex;
	}

	public static int rotateSlabBlock(int slabID, Quaternion rotation) {
		return slabID;
	}
	public static byte[] rotateCSlabBlock(int slabID, int dir, Quaternion rotation) {
		return new byte[] {(byte)slabID, (byte)dir};
	}

	public static VoxelData rotateVoxel(VoxelData block, Quaternion rotation) {
		if (block.block.rotMode == RMode.None && block.block.slabType == 0) return block;
		else if(block.block.rotMode != RMode.None) return new VoxelData(block.block, new byte[] {(byte) rotateDirBlock(block.mainAtr, rotation)});
		else if(block.block.slabType != 0) return new VoxelData(block.block, new byte[] {(byte)rotateSlabBlock(block.mainAtr, rotation)});
		else return new VoxelData(block.block, rotateCSlabBlock(block.data[0], block.data[1], rotation));
	}

	public Quaternion RoundRotation(Quaternion rotation) {
		rotation.ToAngleAxis(out float angle, out Vector3 axis);
		float max = 0;
		int maxIndex = 0;

		if (axis.x >  max) {max = axis.x; maxIndex = 3; }
		if (axis.y >  max) {max = axis.y; maxIndex = 1; }
		if (axis.z >  max) {max = axis.z; maxIndex = 5; }
		if (axis.x < -max) {max = axis.x; maxIndex = 2; }
		if (axis.y < -max) {max = axis.y; maxIndex = 0; }
		if (axis.z < -max) {max = axis.z; maxIndex = 4; }
	
		axis =  VD.dirs[maxIndex];
		angle = Mathf.RoundToInt(angle / 90) * 90;
		return Quaternion.AngleAxis(angle, axis);
	}
}
