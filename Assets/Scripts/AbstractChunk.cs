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
		Block block = GetVoxel(pos);
		
		if (block.type == BType.Voxel) { 
			return VD.slabBounds[GetVoxelAtr(pos)];
		}
		if (block.type == BType.Slope && block.slabType != 0) {
			return VD.slabBounds[GetVoxelData(pos).data[0]];
		}
		if (block.type == BType.Custom) {
			Bounds b = Main.meshTable[block.meshID][render.GetMeshIndex(pos)].bounds;
			if (block.rotMode == RMode.None) return b;

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
	public Block GetVoxel(Vector3Int pos) {
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

	public VoxelData GetVoxelData(Vector3Int pos) {
		Block block = GetVoxel(pos);
		int atr = GetVoxelAtr(pos);
		bool flag = GetFlag(pos);
		if (block.dataSize.Length == 0) return new VoxelData(block, 0, flag);
		else if (block.dataSize.Length == 1) return new VoxelData(block, atr, flag);
		else {
			int p = PosToInt(pos);
			for (int i = atr; i < dataMap.Count; i += 0x001F) {
				if (dataMap[i].pos == p) { return new VoxelData(block, atr, dataMap[i].data, flag); }
			}
			Debug.Log("Missing Blockdata at " + pos.ToString());
			return new VoxelData(block, atr, new byte[block.dataSize.Length], flag);
		}
	}

	public void SetVoxelData(Vector3Int pos, byte[] data) {
		int p = PosToInt(pos);
		if (GetVoxel(pos).dataSize.Length <= 1) {Debug.Log("Blockdata Not Valid at: " + pos.ToString()); return;}
		else {
			for (int i = GetVoxelAtr(pos); i < dataMap.Count; i += 0x001F) {
				if (dataMap[i].pos == p) { dataMap[i] = new ChunkData(p, data); return; }
			}
			SetVoxel(pos, GetVoxel(pos).id, dataMap.Count % 0x001F);
			dataMap.Add(new ChunkData(p, data));
		}
	}
	public void SetVoxelData(Vector3Int pos, int index, byte value) {
		int p = PosToInt(pos);
		for (int i = GetVoxelAtr(pos); i < dataMap.Count; i += 0x011F) {
			if (dataMap[i].pos == p) { dataMap[i].Set(index, value); return; }
		}
	}

	public void ClearVoxelData(Vector3Int pos) {
		int p = PosToInt(pos);

		for (int i = GetVoxelAtr(pos); i < dataMap.Count; i += 0x001F) {
			if (dataMap[i].pos == p) {
				dataMap[i] = new ChunkData(p, null);
				return;
			}
		}
		//SetVoxelAtr(pos, 0);
	}

	//TODO Hashing verwenden
	int PosToInt(Vector3Int v) {
		return v.y + v.x * width.x + v.z * width.z * width.z;
	}

	Vector3Int IntToPos(int i) {
		int y = i % width.y;
		i /= width.y;
		int x = i % width.x;
		i /= width.x;
		int z = i % width.z;
		return new Vector3Int(x, y, z);
	}
}

public struct ChunkData {
	public int pos;
	public byte[] data;

	public ChunkData(int pos, byte[] data) {
		this.pos = pos;
		this.data = data;
	}

	public void Set(int index, byte value) {
		data[index] = value;
	}
}