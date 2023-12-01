using UnityEngine;
using System;

public enum BType { Air, Liquid, Terrain, Rounded, Custom, Voxel, Slope, Combination }
public enum CMode { None, Horizontal, Vertical, Grid, Pipe, Roof, Random }
public enum RMode { None, AllAxis3, YAxis, AllAxis6, Slope }
public enum SMode { UV, UVCutoff, UVAlpha, Triplanar, Water, Invisible }
public enum VECMode { All, Self, SameBlock, SameType, SameCMode, }//VoxelEntityConnectmode

public static class BD {

	//Liste aller Blöcke
	public static Block[] blocks = new Block[] {
        new Block("Air", 0, 0, 0, 0, BType.Air, CMode.None, RMode.None, SMode.Invisible, false, true, new string[0]),
		Block.NewTerrain("Stone", 0),
        Block.NewTerrain("Dirt", 1),
        Block.NewTerrain("Grass", 2),
        Block.NewTerrain("Sand", 3),
    	Block.NewTerrain("Dark Stone", 10),//5

        Block.NewRounded("Rock", 10),
		Block.NewGrid("Cobblestone", 6, 2),
        
		Block.NewPipe("Tree Stem", 4, 0),	
		Block.NewPipe("Tree Branch", 4, 1),
		
		Block.NewCustom("Leaves", 9, 5, CMode.Random, SMode.UVCutoff, false, true, new string[] {"TreeID"}),//10
        Block.NewCustom("Grass Plant", 15, 6, CMode.Random, SMode.UVCutoff, false, true, new string[0]),
        Block.NewPipe("Cactus", 14, 1),
		
		Block.NewBlock("Wood Planks", 5, SMode.Triplanar, false, 0),
        Block.NewBlock("Wood Half Slap", 5, SMode.Triplanar, true, 1),
        Block.NewBlock("Wood Third Slap", 5, SMode.Triplanar, true, 2),//15
        Block.NewSlope("Wood Slope", 5, false, 0),
        Block.NewSlope("Wood Slope Half Slab", 5, false, 1),
        Block.NewSlope("Wood Slope Third Slab", 5, false, 2),

        /*Block.NewBlock("Bricks 2", 8, SMode.Triplanar, false, 0),
        Block.NewBlock("Bricks 1", 7,  SMode.Triplanar,false, 0),//15
        Block.NewBlock("Brick Half Slap", 7, SMode.Triplanar,true, 1),
        Block.NewBlock("Brick Third Slap", 7, SMode.Triplanar,true, 2),
        Block.NewSlope("Brick Slope", 7, false, 0),
        Block.NewSlope("Brick Slope Half Slab", 7, false, 1),
        Block.NewSlope("Brick Slope Third Slab", 7, false, 2),*/
         
        Block.NewGrid("Framing", 5, 3),
	    Block.NewCustomPlus("Frame", 4, 10, 2, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[0]),//20
        Block.NewCustomPlus("Door", 5, 12, 2, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[0]),
        Block.NewCustomPlus("Window", 11, 11, 2, CMode.Vertical, RMode.YAxis, SMode.UVAlpha, true, true, new string[0]),
        Block.NewCustomPlus("Table", 5, 8, 0, CMode.Horizontal, RMode.None, SMode.UVCutoff, true, false, new string[0]),
        Block.NewCustomPlus("Chair", 5, 7, 0, CMode.None, RMode.YAxis, SMode.UVCutoff, true, false, new string[0]),
        Block.NewCustomPlus("Fence", 4, 9, 1, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[0]), //25
		Block.NewCustom("Roof", 13, 13, CMode.Roof, SMode.UVCutoff, true, false, new string[0] ),
		
		Block.NewBlock("Glass", 11, SMode.UVAlpha, true, 0),
  		Block.NewLiquid("Water", 12, 4, 16, 4),
		Block.NewLiquidSource("Water Source", 12),
		
		//letztes 30
		new Block("Block Combination", 0, 1, 0, 0, BType.Combination, CMode.None, RMode.None, SMode.Invisible, true, false, new string[]{"", ""}),

    };	

	//Gibt an wie viele Meshs ein block annehmen kann (im Meshtable)
	public static int[] meshcount = { 192, 192, 63, 64, 2, 3, 3, 1, 16, 16, 15, 16, 16, 81};
	public static int[] roofindexes = {
		 0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
		16, -1, 17, -1, 18, -1, 19, -1, 20, -1, 21, -1, 22, -1, 23, -1,
		24, 25, -1, -1, 26, 27, -1, -1, 28, 29, -1, -1, 30, 31, -1, -1,
		32, -1, -1, -1, 33, -1, -1, -1, 34, -1, -1, -1, 35, -1, -1, -1,
		36, 37, 38, 39, -1, -1, -1, -1, 40, 41, 42, 43, -1, -1, -1, -1, 
		44, -1, 45, -1, -1, -1, -1, -1, 46, -1, 47, -1, -1, -1, -1, -1,
		48, 49, -1, -1, -1, -1, -1, -1, 50, 51, -1, -1, -1, -1, -1, -1,
		52, -1, -1, -1, -1, -1, -1, -1, 53, -1, -1, -1, -1, -1, -1, -1,
		54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, -1,
		62, -1, 63, -1, 64, -1, 65, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		66, 67, -1, -1, 68, 69, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		70, -1, -1, -1, 71, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		72, 73, 74, 75, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		76, -1, 77, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		78, 79, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		80, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,

		/*
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15,
		16,   18,   20,   22,   24,   26,   28,   30,
		32,33,      36,37,      40,41,      44,45,
		48,         52,         56,         60,
		64,65,66,67,68,         72,73,74,75,
		80,   82,               88,   90,
		96,97,                  104,105,
		112,                    120,
		128,129,130,131,132,133,134,135,
		144,  146,  148,  150,
		160,161,    164,165,
		176,        180,
		192,193,194,195,
		208,    210,
		224,225,
		240
		*/
	};

	public static int GetByName(string name) {
		foreach (Block b in blocks) if (b.blockName == name) return b.id;
		Debug.LogWarning("Blockname not found!");
		return 1;
	}
}

public class Block {
	static DynamicWater temp;
    static byte count = 0;

    public byte id;
    public int textureID;	//ID der ersten Textur im TexturArray, Mesh beinhaltet UVs für mehrere Texturen
    public int meshID;		//ID der Zeile der Meshtablelle wo sich die Meshs für den Block befinden

	public float textureScale;

	public Item item;

    public CMode connectMode;
    public RMode rotMode; //Info für Higlight zum platzieren, speichert dir24 Data
    public int slabType; //Speichert slabID
    public BType type;
    public SMode shaderType;

	public string blockName;
    public bool isSolid;		//Hat Collider
	
    public Color32 color;		//Farbe für LOD renderer
    public string[] dataSize;	//Zusätzliche Daten mit bezeichnung (StructureId...)

	public bool isTransparent;			//Wichtig Für Culling
    public float density;				//Höherer Wert überschreibt niedrigen Wert, Rigidbody und Wasser Vehalten, Explosionswiderstand
    public float reactivity;			//Brennbarkeit, Explosionswiderstand

	public Block(string blockName, int textureID, float textureScale, int meshID, int slabType, BType blockType, CMode connectMode, RMode rotMode, SMode shaderType,  bool isSolid, bool isTransparent, string[] dataSize) {
		this.id = count;
		this.textureID = textureID;
		this.meshID = meshID;
		this.textureScale = textureScale;
		this.connectMode = connectMode;
		this.rotMode = rotMode;
		this.slabType = slabType;
		this.type = blockType;
		this.shaderType = shaderType;
		this.blockName = blockName;
		this.isSolid = isSolid;
		this.isTransparent = isTransparent;
		this.dataSize = dataSize;
		count++;
		this.color = Main.colorMap.GetPixel(0, textureID);
	}
	
	

	public static Block NewBlock(string blockName, int texture, SMode shaderMode, bool isTransparent, byte slabs) {
        return new Block(blockName, texture, 1, 0, slabs, BType.Voxel, CMode.None, RMode.None, SMode.UVCutoff, true, isTransparent, slabs == 0 ? new string[0] : new string[] {"SlabID"});
    }
    public static Block NewTerrain(string blockName, int texture) {
        return new Block(blockName, texture, 4, 0, 0, BType.Terrain, CMode.None, RMode.None, SMode.Triplanar, true, false, new string[] { "IsFlat" });
    }
	public static Block NewRounded(string blockName, int texture) {
		return new Block(blockName, texture, 3, 0, 0, BType.Rounded, CMode.None, RMode.None, SMode.Triplanar, true, false, new string[0]);
	}
	public static Block NewCustom(string blockName, int texture, byte meshId, CMode connectMode, SMode shaderMode, bool isSolid, bool isTransparent, string[] data) {
        return new Block(blockName, texture, 1, meshId, 0, BType.Custom, connectMode, RMode.None, shaderMode, isSolid, isTransparent, data);
    }
	public static Block NewCustomPlus(string blockName, int texture, byte meshId, byte slabType, CMode connectMode, RMode rotateMode, SMode shaderMode, bool isSolid, bool isTransparent, string[] data) {
		return new Block(blockName, texture, 1, meshId, slabType, BType.Custom, connectMode, rotateMode, shaderMode, isSolid, isTransparent, Main.Concat( slabType != 0 ? new string[] { "SlabID" } : new string[] {"Rotation"}, data));
	}
	public static Block NewSlope(string blockName, int texture, bool isTransparent, byte slabs) {
        return new Block(blockName, texture, 1, 1, slabs, BType.Slope, CMode.None, RMode.Slope, SMode.UVCutoff, true, isTransparent, slabs == 0 ? new string[] { "Direction12" } : new string[] { "SlabID", "Direction12" });
    }
    public static Block NewGrid(string blockName, int texture, int meshid) {
        return new Block(blockName, texture, 2, meshid, 0, BType.Custom, CMode.Grid, RMode.None, SMode.UVCutoff, true, false, new string[0]);
	}
    public static Block NewPipe(string blockName, int texture, int meshid) {
        return new Block(blockName, texture, 1, meshid, 0, BType.Custom, CMode.Pipe, RMode.AllAxis3, SMode.UVCutoff, true, false, new string[] { "Direction3" });
    }
    public static Block NewLiquid(string blockName, int texture, int flowAmount, int layer, int speed) {
        temp = new DynamicWater(blockName, texture, flowAmount, layer, speed);
		return temp;
    }
	public static Block NewLiquidSource(string blockName, int texture) {
		return new StaticWater(blockName, texture, temp);
	}


	//Durchschnittsfarbe der Textur
	public static Color GetTextureColor(Color[] pixels) {
		int x = 0;
		float r = 0f, g = 0f, b = 0f;

		foreach (Color c in pixels) {
			if (c.a != 0) {
				r += c.r;
				g += c.g;
				b += c.b;
				x++;
			}
		}
		return new Color(r / x, g / x, b / x);
	}

    public virtual bool OnBlockUpdate(Chunk chunk, Vector3Int pos) {
        return false;
    }

    public virtual void OnRandomTick() { }
    
    public virtual void OnUse() {

    }
}
	//byte meshtype
	//ID Bez(Bsp) Zeilenlänge der Meshtabelle
	// 0: Kein Connect 1
	// 1  ConnectXZ4(Zaun) 14
	// 2  ConnectXZ8(Tisch) 9                  
	// 3  ConnectY8(Fenster) 9
	// 4  ConnectXYZ6(Pipe) 192
	// 5  ConnectXYZ26(Grid) 128

	//byte rotMode
	//ID Bez(Bsp)
	// 0 kein Rotate
	// 1  Rotate 3x(Pipe)
	// 2  Rotate 4x(Stuhl)
	// 3  Rotate 6x(Custom)
	// 4  Rotate 12x(Slope)
	// 5  Rotate 4xslab(Door)

	//byte slabMode
	// 0 Kein
	// 1 Halfslab
	// 2 Thirdslab
	// 3 Pillar?


	//bool mode   Ein block hat entweder meshType oder rotmode

	//byte blockType
	//ID Bez (attribute)            mode: true (zusatzaribute) / false (zusatzaribute)
	// 0 Rendert Nicht
	// 1 Flüssig (textur)   mode: static / dynamic                                      MainAtr height (16)
	// 2 Terrain (textur)           mode: terrain / rounded                             MainAtr grounded (1)
	// 3 Voxel (textur slabmode)    mode: full voxel / custom voxel (rotmode meshid)    MainAtr slabmode  = 0? leer / rotation(24) : slabID (17) / data[0] slabID(17), data[1] rotation(24)
	// 4 Custom (textur)            mode: rot (rotmode) / connect (meshtype)            Mainatr rotation (24) / leer
	// 5 Custom Slab (textur slabmode meshtype? ) mode: Rotate3X : Rotate2X             Mainatr CslabId (15 / 30?)

	//byte shaderMode
	// 0 Single Color 
	// 1 UV Shader      Benutze UVs aus Mesh
	// 2 UV Cutoff 
	// 3 UV Transparent 
	// 4 Triplanar      Terrain shader
	// 5 Water          

	