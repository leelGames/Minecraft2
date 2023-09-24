using UnityEngine;

public class Chunk : AChunk {
	public Vector2Int coords;
	public World world;

	//1: objekt erstellt 2: Terrain generiert 3: struktur generiert 4: Mesh generiert 5: Mesh erstellt
	//für state 3+ müssen beachbarte chunks state 2 haben
	public int state = 0;
	public int level;

	public ChunkgenData[,] genData = new ChunkgenData[VD.ChunkWidth, VD.ChunkWidth];

	public Vector3Int Position {
		get { return new Vector3Int(coords.x * VD.ChunkWidth, 0, coords.y * VD.ChunkWidth); }
	}

	public void Init(Vector2Int coord, World w) {
		Init(VD.ChunkWidth, VD.ChunkHeight, VD.ChunkWidth);
		coords = coord;
		world = w;

		gameObject.transform.SetParent(world.transform);
		gameObject.transform.position = Position;
		gameObject.name = "Chunk " + coords.x + ", " + coords.y;
	}

	//Benachbarte Chunks generieren um Strukturgeneration zu ermöglichen
	public void GenerateSurounding() {
		for (int i = 0; i < 8; i++) {
			Chunk nextchunk = world.GetChunkC(coords + VD.chunkChecks[i]);
			if (nextchunk.state < 2) {
				nextchunk.Generate();
			}
		}
		state = 2;
	}

	//füllt Chunk mit Blöcken
	public void Generate() {
		for (int x = 0; x < VD.ChunkWidth; x++) {
			for (int z = 0; z < VD.ChunkWidth; z++) {
				genData[x, z] = new ChunkgenData();
				genData[x, z].TerrainData = world.gen.GenerateTerrain(new Vector2Int(x + Position.x, z + Position.z));
				for (int y = 0; y <= genData[x, z].terrainheight || y <= world.gen.sealevel; y++) {
					if (GetVoxel(x, y, z) == 0) SetVoxel(x, y, z, world.gen.GenerateBlock(new Vector3Int(x, y, z) + Position, genData[x, z].TerrainData));
				}
			}
		}
	}

	//Benachbarte Chunks strukturieren um update zu ermöglichen
	public void StrucktureSurrounding() {
		for (int i = 0; i < 8; i++) {
			Chunk nextchunk = world.GetChunkC(coords + VD.chunkChecks[i]);
			
			if (nextchunk.state < 3) {
				nextchunk.GenerateStructures();
			}
		}
		state = 3;
	}

	public void GenerateStructures() {
		for (int x = 0; x < VD.ChunkWidth; x++) {
			for (int z = 0; z < VD.ChunkWidth; z++) {
				genData[x, z].structureid = world.gen.GenerateStructure(new Vector2Int(x + Position.x, z + Position.z), genData[x, z].TerrainData);
			}
		}
	}

	public void CalcHeightData() {
		int height;
		for (int x = 0; x < VD.ChunkWidth; x++) {
			for (int z = 0; z < VD.ChunkWidth; z++) {
				height = GetMaxHeight(x, z);
				genData[x, z].maxheight = height;
				genData[x, z].minheight = genData[x, z].terrainheight;
				genData[x, z].lodcolor = (byte) GetVoxel(x, height, z);
			}
		}
	}

	int GetMaxHeight(int x, int z) {
		int terrainheight = genData[x, z].terrainheight;
		int height = terrainheight;
		for (int y = terrainheight; y < terrainheight + 64 && y < VD.ChunkHeight; y++) {
			if (GetVoxel(x, y, z) != 0) height = y;
		}
		return height;
	}
		
	//Generiert das Mesh des Chunks
	public override void UpdateChunk(UpdateEvent e) {
		if (e.lod != -1) level = e.lod;
		else e.lod = level;

		for (int x = 0; x < VD.ChunkWidth; x++) {
			for (int z = 0; z < VD.ChunkWidth; z++) {
				for (int y = genData[x, z].minheight - 3; y < genData[x, z].maxheight + 2; y++) {
					render.UpdateMesh(e, new Vector3Int(x, y, z));
				}
			}
		}
		state = 4;
	}

	public void SetLod(int lod) {
		if (lod != -1) level = lod;
		if (lod == 0) {
			meshRenderer.materials = new Material[] { Main.materials[0], Main.materials[1], Main.materials[2], Main.materials[3] };
			meshFilter.mesh = render.mesh;
		} else {
			meshRenderer.materials = new Material[] { Main.materials[4], Main.materials[5] };
			meshFilter.mesh = render.lodmesh;
		}
	}

	//Effizentes Chunkübergeifendes blockprüfen
	public override Block CheckBlock(Vector3Int pos) {

		if (!IsVoxelInChunk(pos)) {
			return world.GetBlock(pos + Position);
		}
		return GetBlock(pos);
	}

	public override int CheckBlockAtr(Vector3Int pos) {

		if (!IsVoxelInChunk(pos)) {
			return world.GetVoxelData(pos + Position).mainAtr;
		}
		return GetVoxelAtr(pos);
	}

	public override Bounds CheckBounds(Vector3Int pos) {

		if (!IsVoxelInChunk(pos)) {
			return world.GetBounds(pos + Position);
		}
		return GetBounds(pos);
	}

}

/*if (coords == new Vector2Int(0, 0)) activeSubs[genData[0, 0].terrainData.terrainheight / VD.subChunks] = true;
		for (int i = 0; i < VD.subChunks; i++) {
			if (activeSubs[i]) {
				if (i + 1 < VD.subChunks) CheckSubchunk(i + 1);
				if (i - 1 >= 0) CheckSubchunk(i - 1);
			}
		}
		for (int i = 0; i < VD.subChunks; i++) {
			if (activeSubs[i]) {
				for (int j = 0; j < 8; j += 2) {
					world.GetChunkC(coords + VD.chunkChecks[j]).CheckSubchunk(i);
				}
			}
		}
		int inc = VD.ChunkHeight / 2;
		int max = inc;
		int min = inc;
		while (inc >= 2) {
			inc /= 2;
			if (GetVoxel(x, max, z) == 0) max -= inc;
			else max += inc;
			if (GetVoxel(x, min, z) == 0) min += inc;
			else min -= inc;
		}
		genData[x, z].LodData = new LodgenData()

	}

	public bool CheckSubchunk(int sub) {
		if (activeSubs[sub]) return true;
		bool block = BD.blocks[GetVoxel(0, sub * VD.ChunkWidth, 0)].type == 0;
		for (int x = 0; x < VD.ChunkWidth; x++) {
			for (int z = 0; z < VD.ChunkWidth; z++) {
				for (int y = sub * VD.ChunkWidth; y < (sub + 1) *  VD.ChunkWidth; y++) {
					if ((BD.blocks[GetVoxel(x, y, z)].type == 0) != block) {
						activeSubs[sub] = true;
						return true;
					}
				}
			}
		}
		return false;
	}
*/