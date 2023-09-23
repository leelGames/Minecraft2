
public static class BiomeData {
	public static readonly Tree[] trees = {
		new Tree("Grass", "Tree Stem", "Tree Branch", "Leaves", 10, 0.8f, 15, 3), //Default Baum
		new Tree("Dirt", "Tree Stem", "Tree Branch", "Leaves", 10, 0.8f, 25, 5) //Groser Baum
	};

	public static readonly Plant[] plants = {
		new Plant("Sand", "Cactus", 10, 0.8f, 4),		//Kaktus	
		new Plant("Grass", "Grass Plant", 10, 0.8f, 1),  //Gras
		new Plant("Grass", "Grass Plant", 10, 0.5f, 2)	//Dense Gras
	};

	public static readonly Rock[] rocks = {
		new Rock("Rock", 10, 0.85f, 10),
		new Rock("Cobblestone", 10, 0.8f, 6),

	};
	public static readonly Lode[] lodes = {
		new Lode("Dirt", 30, 60, 0.05f, 0.5f),	//Dirt
		new Lode("Stone", 50, 100, 0.03f, 0.5f)	//Stone
	};
	
	public static readonly TerrainB[] terrains = {
		new TerrainB(96, 0.05f, 24, 0.3f, 1),		//Hills
		new TerrainB(48, 0.05f, 24, 0.5f, 3),		//Dunes
		new TerrainB(48, 0.01f, 4, 0.03f, 1),		//Flatland
		new TerrainB(128, 0.1f, 48, 0.5f, 2),		//Mountains
		new TerrainB(0, 0.05f, 8, 0.03f, 1)			//Ocean
	};

	public static readonly TypeB[] types = {
		new TypeB("Grass", "Dirt", 0.2f, 0.4f, lodes, new Tree[] {trees[0]}, new Plant[]{ plants[0]}),			//Default
		new TypeB("Sand", "Sand", 0.2f, 0.4f, new Lode[] {lodes[1]}, new Tree[0], new Plant[]{ plants[0]}),		//Desert
		new TypeB("Grass", "Dirt", 0.8f, 0.1f, new Lode[0], new Tree[0], new Plant[]{ plants[2]}),				//Fields
		new TypeB("Dirt", "Dirt", 0.8f, 0.05f, new Lode[0], new Tree[] {trees[1] }, new Plant[]{ plants[1]}),	//Forrest
		new TypeB("Stone", "Stone", 1f, 1f, new Lode[0], new Tree[0], new Plant[0]),							//Mountains
		new TypeB("Dark Stone", "Stone", 1f, 1f, lodes, new Tree[0], new Plant[0])								//Ocean (Temp)
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

	public float treeZoneScale;
	public float treeZoneThresh;
	public Lode[] lodes;
	public Tree[] trees;
	public Plant[] plants;
	public int surfaceBlock;
	public int subSurfaceBlock;

	public TypeB(string surfaceBlock, string subSurfaceBlock, float treeZoneScale, float treeZoneThresh, Lode[] lodes, Tree[] trees, Plant[] plants) {
		this.surfaceBlock = BD.GetByName(surfaceBlock);
		this.subSurfaceBlock = BD.GetByName(subSurfaceBlock);
		this.treeZoneScale = treeZoneScale;
		this.treeZoneThresh = treeZoneThresh;
		this.lodes = lodes;
		this.trees = trees;
		this.plants = plants;
	}
}

public struct Tree {
	public int groundBlock;
	public int stemBlock;
	public int branchBlock;
	public int leafBlock;
	public float treePlacementScale;
	public float treePlacementThresh;
	public int maxTreeHeight;
	public int minTreeHeight;

	public Tree(string groundBlock, string stemBlock, string branchBlock, string leafBlock, float treePlacementScale, float treePlacementThresh, int maxTreeHeight, int minTreeHeight) {
		this.groundBlock = BD.GetByName(groundBlock);
		this.stemBlock = BD.GetByName(stemBlock);
		this.branchBlock = BD.GetByName(branchBlock);
		this.leafBlock = BD.GetByName(leafBlock);
		this.treePlacementScale = treePlacementScale;
		this.treePlacementThresh = treePlacementThresh;
		this.maxTreeHeight = maxTreeHeight;
		this.minTreeHeight = minTreeHeight;
	}
}

public struct Plant {
	public int groundBlock;
	public int block;
	public float plantPlacementScale;
	public float plantPlacementThresh;
	public int maxHeight;

	public Plant(string groundBlock, string block, float plantPlacementScale, float plantPlacementThresh, int maxHeight) {
		this.groundBlock = BD.GetByName(groundBlock);
		this.block = BD.GetByName(block);
		this.plantPlacementScale = plantPlacementScale;
		this.plantPlacementThresh = plantPlacementThresh;
		this.maxHeight = maxHeight;
	}
}

public struct Rock {
	public int block;
	public float rockPlacementScale;
	public float rockPlacementThresh;
	public int maxSize;

	public Rock(string block, float rockPlacementScale, float rockPlacementThresh, int maxSize) {
		this.block = BD.GetByName(block);
		this.rockPlacementScale = rockPlacementScale;
		this.rockPlacementThresh = rockPlacementThresh;
		this.maxSize = maxSize;
	}
}

public struct Lode {
	public int block;
	public int minHeight;
	public int maxHeight;
	public float scale;
	public float thres;

	public Lode(string block, int minHeight, int maxHeight, float scale, float thres) {
		this.block = BD.GetByName(block);
		this.minHeight = minHeight;
		this.maxHeight = maxHeight;
		this.scale = scale;
		this.thres = thres;
	}
}


