
public static class BiomeData {
	public static readonly Tree[] trees = {
		new Tree(3, 7, 35, 22, 10, 0.8f, 15, 3), //Default Baum
		new Tree(2, 7, 35, 22, 10, 0.8f, 25, 5) //Groser Baum
	};

	public static readonly Plant[] plants = {
		new Plant(4, 21, 10, 0.8f, 4),	//Kaktus	
		new Plant(3, 23, 10, 0.8f, 1),  //Gras
		new Plant(3, 23, 10, 0.5f, 2)	//Dense Gras
	};

	public static readonly Rock[] rocks = {
		new Rock(34, 10, 0.85f, 10),
		new Rock(6, 10, 0.8f, 6),

	};
	public static readonly Lode[] lodes = {
		new Lode(2, 30, 60, 0.05f, 0.5f),	//Dirt
		new Lode(1, 50, 100, 0.03f, 0.5f)	//Stone
	};
	
	public static readonly TerrainB[] terrains = {
		new TerrainB(96, 0.05f, 24, 0.3f, 1),		//Hills
		new TerrainB(48, 0.05f, 24, 0.5f, 3),		//Dunes
		new TerrainB(48, 0.01f, 4, 0.03f, 1),		//Flatland
		new TerrainB(128, 0.1f, 48, 0.5f, 2),		//Mountains
		new TerrainB(0, 0.05f, 8, 0.03f, 1)			//Ocean
	};

	public static readonly TypeB[] types = {
		new TypeB(lodes, 0.2f, 0.4f, new Tree[] {trees[0]}, 3, 2, new Plant[]{ plants[0]}),				//Default
		new TypeB(new Lode[] {lodes[1]}, 0.2f, 0.4f, new Tree[] {}, 4, 4, new Plant[]{ plants[0]}),		//Desert
		new TypeB(new Lode[]{}, 0.8f, 0.1f, new Tree[] {}, 3, 2, new Plant[]{ plants[2]}),				//Fields
		new TypeB(new Lode[]{ }, 0.8f, 0.05f, new Tree[] {trees[1] }, 2, 2, new Plant[]{ plants[1]}),	//Forrest
		new TypeB(new Lode[]{ }, 1f, 1f, new Tree[] {}, 1, 1, new Plant[]{ }),							//Mountains
		new TypeB(lodes, 1f, 1f, new Tree[]{ }, 5, 1, new Plant[]{ })									//Ocean (Temp)
	};

	public static readonly Biome[] biomes = {
		new Biome("Default Biome", terrains[0], types[0], false),	//Default Biome
		new Biome("Desert", terrains[1], types[1], false),			//Desert
		new Biome("Flat Fields", terrains[2], types[2], false),		//Default Flat Fields
		new Biome("Mountains", terrains[3], types[4], false),		//Berge
		new Biome("Forrest", terrains[0], types[3], false),			//Wald
		new Biome("Ocean", terrains[4], types[5], true)				//Ocean Test
	};
}

public class Biome {

	public string name;
	public TerrainB terrain;
	public TypeB type;
	public bool isOcean;

	public Biome(string name, TerrainB terrain, TypeB type, bool isOcean) {
		this.name = name;
		this.terrain = terrain;
		this.type = type;
		this.isOcean = isOcean;
	}
}

public struct TerrainB {

	public int heightL1;
	public float scaleL1;
	public int heightL2;
	public float scaleL2;
	public int stretch;

	public TerrainB(int heightL1, float scaleL1, int heightL2, float scaleL2, int stretch) {
		this.heightL1 = heightL1;
		this.scaleL1 = scaleL1;
		this.heightL2 = heightL2;
		this.scaleL2 = scaleL2;
		this.stretch = stretch;
	}
}

public struct TypeB {

	public Lode[] lodes;

	public float treeZoneScale;
	public float treeZoneThresh;
	public Tree[] trees;
	public Plant[] plants;
	public byte surfaceBlock;
	public byte subSurfaceBlock;

	public TypeB(Lode[] lodes, float treeZoneScale, float treeZoneThresh, Tree[] trees, byte surfaceBlock, byte subSurfaceBlock, Plant[] plants) {
		this.lodes = lodes;
		this.treeZoneScale = treeZoneScale;
		this.treeZoneThresh = treeZoneThresh;
		this.trees = trees;
		this.surfaceBlock = surfaceBlock;
		this.subSurfaceBlock = subSurfaceBlock;
		this.plants = plants;
	}
}

public struct Tree {

	public byte groundBlock;
	public byte stemBlock;
	public byte branchBlock;
	public byte leafBlock;
	public float treePlacementScale;
	public float treePlacementThresh;
	public int maxTreeHeight;
	public int minTreeHeight;

	public Tree(byte groundBlock, byte stemBlock, byte branchBlock, byte leafBlock, float treePlacementScale, float treePlacementThresh, int maxTreeHeight, int minTreeHeight) {
		this.groundBlock = groundBlock;
		this.stemBlock = stemBlock;
		this.branchBlock = branchBlock;
		this.leafBlock = leafBlock;
		this.treePlacementScale = treePlacementScale;
		this.treePlacementThresh = treePlacementThresh;
		this.maxTreeHeight = maxTreeHeight;
		this.minTreeHeight = minTreeHeight;
	}
}

public struct Plant {

	public byte groundBlock;
	public byte block;
	public float plantPlacementScale;
	public float plantPlacementThresh;
	public int maxHeight;

	public Plant(byte groundBlock, byte block, float plantPlacementScale, float plantPlacementThresh, int maxHeight) {
		this.groundBlock = groundBlock;
		this.block = block;
		this.plantPlacementScale = plantPlacementScale;
		this.plantPlacementThresh = plantPlacementThresh;
		this.maxHeight = maxHeight;
	}
}

public struct Rock {
	public byte block;
	public float rockPlacementScale;
	public float rockPlacementThresh;
	public int maxSize;

	public Rock(byte block, float rockPlacementScale, float rockPlacementThresh, int maxSize) {
		this.block = block;
		this.rockPlacementScale = rockPlacementScale;
		this.rockPlacementThresh = rockPlacementThresh;
		this.maxSize = maxSize;
	}
}

public struct Lode {

	public byte block;
	public int minHeight;
	public int maxHeight;
	public float scale;
	public float thres;

	public Lode(byte block, int minHeight, int maxHeight, float scale, float thres) {
		this.block = block;
		this.minHeight = minHeight;
		this.maxHeight = maxHeight;
		this.scale = scale;
		this.thres = thres;
	}
}


