using System.Collections.Generic;
using UnityEngine;


public abstract class AChunk : MonoBehaviour{
	public VoxelRenderer render;

	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;
	public MeshCollider meshCollider;

	protected ushort[,,] voxelMap; //Speichert alle BlockIDs im Chunk

	//	 mainatr   flag         id 
	//    ____|____ | __________|________
	// 0b 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0

	public List<ChunkData> dataMap;

	Vector3Int width;

	private bool isActive = false;
	public bool Active {
		get { return isActive; }
		set { 
			isActive = value;
			gameObject.SetActive(value); 
		}
	}

	//Gameobject initialisieren
	public void Init(int wx, int wy, int wz) {	
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshCollider = gameObject.AddComponent<MeshCollider>();

		//meshRenderer.materials = Main.materials;
		render = new VoxelRenderer(this);

		width = new Vector3Int(wx, wy, wz);
		voxelMap = new ushort[wx, wy, wz]; 						 
		dataMap = new();
	}

	public abstract Block CheckBlock(Vector3Int pos);
	public abstract int CheckBlockAtr(Vector3Int pos);
	public abstract Bounds CheckBounds(Vector3Int pos);
	public abstract void UpdateChunk(UpdateEvent e);

	public bool IsVoxelInChunk(Vector3Int pos) {
		return pos.x >= 0 && pos.x < width.x && pos.y >= 0 && pos.y < width.y && pos.z >= 0 && pos.z < width.z;
	}

	public Bounds GetBounds(Vector3Int pos) {
		Block block = GetBlock(pos);
		
		if (block.type == BType.Voxel) { 
			return VD.slabBounds[GetVoxelAtr(pos)];
		}
		if (block.type == BType.Slope) {
			return VD.slabBounds[block.slabType == 0 ? 0 : GetData(pos)[0]];
		}
		if (block.type == BType.Custom) {
			Bounds b = Main.meshTable[block.meshID][render.GetMeshIndex(pos)].bounds;
			if (block.rotMode == RMode.None || block.connectMode == CMode.Pipe) return b;

			int thisAtr = GetVoxelAtr(pos);
			//Bounds rotiern is kompliziert
			Vector3 min;
			Vector3 max;
			if (block.slabType == 0) {
				min = VoxelRenderer.RotateVert(b.min, thisAtr);
				max = VoxelRenderer.RotateVert(b.max, thisAtr);
			}
			else {
				min = VoxelRenderer.RotateVert(b.min + new Vector3(0f, 0f, thisAtr % 3 / 3f), thisAtr / 3);
				max = VoxelRenderer.RotateVert(b.max + new Vector3(0f, 0f, thisAtr % 3 / 3f), thisAtr / 3);
			}
			float t;
			if (min.x > max.x) { t = min.x; min.x = max.x; max.x = t; }
			if (min.y > max.y) { t = min.y; min.y = max.y; max.y = t; }
			if (min.z > max.z) { t = min.z; min.z = max.z; max.z = t; }
			//max.y -= min.y;
			//min.y = 0;
			b.SetMinMax(min, max);
			return b;
		}
		if (block.type == BType.Liquid) {
			Bounds b = new Bounds();
			b.SetMinMax(Vector3.zero, new Vector3(1f,GetVoxelAtr(pos) / 16f, 1f));
			return b;
		}
		return VD.slabBounds[0];
	}

	protected int GetVoxel(int x, int y, int z) {
		return voxelMap[x, y, z] & 0x03FF;
	}
	public int GetVoxel(Vector3Int pos) {
		return voxelMap[pos.x, pos.y, pos.z] & 0x03FF; 
	}
	public Block GetBlock(Vector3Int pos) {
		return BD.blocks[voxelMap[pos.x, pos.y, pos.z] & 0x03FF]; 
	}
	public int GetVoxelAtr(int x, int y, int z) {
		return voxelMap[x, y, z] >> 11;
	}
	public int GetVoxelAtr(Vector3Int pos) {
		return voxelMap[pos.x, pos.y, pos.z] >> 11;
	}
	protected void SetVoxel(int x, int y, int z, int id) {
		voxelMap[x, y, z] = (ushort) id;
	}
	public virtual void SetVoxel(Vector3Int pos, int id) {
		voxelMap[pos.x, pos.y, pos.z] = (ushort) id;
	}
	public virtual void SetVoxel(Vector3Int pos, int id, int atr) {
		voxelMap[pos.x, pos.y, pos.z] = (ushort)((atr << 11) | id);
	}

	public void SetFlag(Vector3Int pos, bool f) {
		if (f) voxelMap[pos.x, pos.y, pos.z] |= 0x0400;
		else voxelMap[pos.x, pos.y, pos.z] &= 0xFBFF;
	}

	public bool GetFlag(Vector3Int pos) {
		return (voxelMap[pos.x, pos.y, pos.z] & 0x0400) == 0x400;
	}
	
	public byte[] GetData(Vector3Int pos) {
		Block block = GetBlock(pos);
		int atr = GetVoxelAtr(pos);
	
		if (block.dataSize.Length == 0) return new byte[0];
		else if (block.dataSize.Length == 1) return new byte[] {(byte) atr};
		else {
			int hash = chunkHash(pos);
			for (int i = atr; i < dataMap.Count; i += 32) {
				if (dataMap[i].hash == hash) { return dataMap[i].data; }
			}
			Debug.Log("Missing Blockdata at " + pos.ToString());
			return new byte[0];
		}
	}

	public void SetVoxel(Vector3Int pos, int id, byte[] data) {
		int hash = chunkHash(pos);
		if (data == null || data.Length < 2) {Debug.Log("Invalid Chunk Data: " + data.Length); return;}
		
		SetVoxel(pos, id, dataMap.Count % 32);
		dataMap.Add(new ChunkData(hash, data));
	}

	public void CopyVoxel(Vector3Int pos, VoxelData data) {
		if (data.block.dataSize.Length == 0) SetVoxel(pos, data.block.id);
		else if (data.block.dataSize.Length == 1) SetVoxel(pos, data.block.id, data.mainAtr);
		else SetVoxel(pos, data.block.id, data.data);
	}

	/*public void ClearVoxelData(Vector3Int pos) {
		int hash = chunkHash(pos);
		if (GetBlock(pos).dataSize.Length <= 1) return;

		for (int i = GetVoxelAtr(pos); i < dataMap.Count; i += 32) {
			if (dataMap[i].hash == hash) {
				dataMap[i] = new ChunkData(hash, null);
				
				return;
			}
		}
	}*/

	int chunkHash(Vector3Int v) {
		return v.y + v.x * width.x + v.z * width.x * width.z;
	}
}

public struct ChunkData {
	public int hash;
	public byte[] data;

	public ChunkData(int hash, byte[] data) {
		this.hash = hash;
		this.data = data;
	}

	/*public void Set(int index, byte value) {
		data[index] = value;
	}*/
}