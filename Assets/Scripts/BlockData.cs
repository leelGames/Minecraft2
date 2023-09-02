using UnityEngine;

public enum BType { Air, Liquid, Terrain, Rounded, Custom, Voxel, CustomSlab }
public enum CMode { None, Horizontal, Vertical, Grid, Pipe }
public enum RMode { None, AllAxis3, YAxis, AllAxis6, Slope }
public enum SMode { UV, UVCutoff, UVAlpha, Triplanar, Water }
public enum VECMode { All, Self, SameBlock, SameType, SameCMode, }//VoxelEntityConnectmode


	

public static class BD {

	//Liste aller Bl�cke
	public static Block[] blocks = new Block[] {
        new Block("Air", 0, 0, 0, 0, false, BType.Air, CMode.None, RMode.None, SMode.Triplanar, false, true, new string[0]),
		Block.NewTerrain("Stone", 0),
        Block.NewTerrain("Dirt", 1),
        Block.NewTerrain("Grass", 2),
        Block.NewTerrain("Sand", 3),
        Block.NewTerrain("Dark Stone", 10),//5

        Block.NewGrid("Cobblestone", 6, 2),
        Block.NewPipe("Tree Stem", 4, 0),
		

        Block.NewBlock("Wood Planks", 5, SMode.Triplanar, false, 0),
        Block.NewBlock("Wood Half Slap", 5, SMode.Triplanar, true, 1),
        Block.NewBlock("Wood Third Slap", 5, SMode.Triplanar, true, 2),//10
        Block.NewSlope("Wood Slope", 5, false, 0),
        Block.NewSlope("Wood Slope Half Slab", 5, false, 1),
        Block.NewSlope("Wood Slope Third Slab", 5, false, 2),

        Block.NewBlock("Bricks 2", 8, SMode.Triplanar, false, 0),
        Block.NewBlock("Bricks 1", 7,  SMode.Triplanar,false, 0),//15
        Block.NewBlock("Brick Half Slap", 7, SMode.Triplanar,true, 1),
        Block.NewBlock("Brick Third Slap", 7, SMode.Triplanar,true, 2),
        Block.NewSlope("Brick Slope", 7, false, 0),
        Block.NewSlope("Brick Slope Half Slab", 7, false, 1),
        Block.NewSlope("Brick Slope Third Slab", 7, false, 2),//20
        
        Block.NewPipe("Cactus", 14, 1),
        Block.NewCustom("Leaves", 9, 3, CMode.None, RMode.None, SMode.UVCutoff, false, true, new string[] {"TreeID"}),
        Block.NewCustom("Grass", 15, 4, CMode.None, RMode.None, SMode.UVCutoff, false, true, new string[0]),
        new DynamicWater(4, 16, 4),
        new StaticWater(24),//25
        Block.NewCustom("Roof", 13, 11, CMode.Horizontal, RMode.None, SMode.UVCutoff, true, false, new string[] {"Direction4"}),
        Block.NewGrid("Framing", 5, 13),
	   // Block.NewCustomSlab("Doorframe", 4, 7, 2, RMode.YAxis, SMode.UVCutoff, true, false, new string[] {"CustomslabID"}),
        Block.NewCustom("Door", 5, 8, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[] {"Direction4"}),
        Block.NewCustom("Windowframe", 4, 9, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[] { "Direction4" }),
        Block.NewCustom("Window", 11, 10, CMode.Vertical, RMode.YAxis, SMode.UVAlpha, true, true, new string[] {"Direction4"}),//30
        Block.NewCustom("Table", 5, 6, CMode.Horizontal, RMode.AllAxis6, SMode.UVCutoff, true, false, new string[] {"Rotation"}),
        Block.NewCustom("Chair", 5, 5, CMode.None, RMode.YAxis, SMode.UVCutoff, true, false, new string[] {"Direction4"}),
        Block.NewBlock("Glass", 11, SMode.UVAlpha, true, 0),
        Block.NewRounded("Rock", 10),
		Block.NewPipe("Tree Branch", 4, 1),
		Block.NewCustom("Fence", 4, 12, CMode.Vertical, RMode.YAxis, SMode.UVCutoff, true, false, new string[] { "CustomslabID" }),

		//TODO custom rotierung integrieren (RMode nur für platzierung)
		//Customslabs integrieren
    };	

	//Gibt an wie viele Meshs ein block annehmen kann (im Meshtable)
	public static int[] meshcount = { 192, 192, 63, 3, 3, 1, 16, 1, 16, 15, 16, 16, 16, 64};
}

public class Block {
    static byte count = 0;

    public byte id;
    public int textureID;	//ID der ersten Textur im TexturArray, Mesh beinhaltet UVs für mehrere Texturen
    public int meshID;		//ID der Zeile der Meshtablelle wo sich die Meshs für den Block befinden

	public float textureScale;

    public CMode connectMode;
    public RMode rotMode;
    public int slabType;
    public bool mode;
    public BType type;
    public SMode shaderType;

	public string blockName;
    public bool isSolid;		//Hat Collider
	
    public Color32 color;		//Farbe für LOD renderer
    public string[] dataSize;	//Zusätzliche Daten mit bezeichnung (StructureId...)

	public bool isTransparent;			  //   
    public float density;				//Höherer Wert überschreibt niedrigen Wert, Rigidbody und Wasser Vehalten, Explosionswiderstand
    public float reactivity;			//Brennbarkeit, Explosionswiderstand

	public Block(string blockName, int textureID, float textureScale, int meshID, int slabType, bool mode, BType blockType, CMode connectMode, RMode rotMode, SMode shaderType,  bool isSolid, bool isTransparent, string[] dataSize) {
		this.id = count;
		this.textureID = textureID;
		this.meshID = meshID;
		this.textureScale = textureScale;
		this.connectMode = connectMode;
		this.rotMode = rotMode;
		this.slabType = slabType;
		this.mode = mode;
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
        return new Block(blockName, texture, 1, 0, slabs, false, BType.Voxel, CMode.None, RMode.None, SMode.UVCutoff, true, isTransparent, new string[] {"SlabID"});
    }
    public static Block NewTerrain(string blockName, int texture) {
        return new Block(blockName, texture, 4, 0, 0, false, BType.Terrain, CMode.None, RMode.None, SMode.Triplanar, true, false, new string[] { "IsFlat" });
    }
	public static Block NewRounded(string blockName, int texture) {
		return new Block(blockName, texture, 3, 0, 0, false, BType.Rounded, CMode.None, RMode.None, SMode.Triplanar, true, false, new string[0]);
	}
	public static Block NewCustom(string blockName, int texture, byte meshId, CMode connectMode, RMode rotateMode, SMode shaderMode, bool isSolid, bool isTransparent, string[] data) {
        return new Block(blockName, texture, 1, meshId, 0, false, BType.Custom, connectMode, rotateMode, shaderMode, isSolid, isTransparent, data);
    }
	public static Block NewCustomSlab(string blockName, int texture, byte meshId, byte slabType, RMode rotateMode, SMode shaderMode, bool isSolid, bool isTransparent, string[] data) {
		return new Block(blockName, texture, 1, meshId, slabType, false, BType.CustomSlab, CMode.None, rotateMode, shaderMode, isSolid, isTransparent, data);
	}
	public static Block NewSlope(string blockName, int texture, bool isTransparent, byte slabs) {
        return new Block(blockName, texture, 1, 1, slabs, true, BType.Voxel, CMode.None, RMode.Slope, SMode.UVCutoff, true, isTransparent, slabs == 0 ? new string[] { "SlabID" } : new string[] { "SlabID", "Direction12" });
    }
    public static Block NewGrid(string blockName, int texture, int meshid) {
        return new Block(blockName, texture, 2, meshid, 0, true, BType.Custom, CMode.Grid, RMode.None, SMode.UVCutoff, true, false, new string[0]);
	}
    public static Block NewPipe(string blockName, int texture, int meshid) {
        return new Block(blockName, texture, 1, meshid, 0, true, BType.Custom, CMode.Pipe, RMode.AllAxis3, SMode.UVCutoff, true, false, new string[] { "Direction3" });
    }
    public static Block NewLiquid(string blockName, int texture, bool isDynamic) {
        return new Block(blockName, texture, 4, 0, 0, !isDynamic, BType.Liquid, CMode.None, RMode.None, SMode.UVAlpha, false, true, new string[] { "Height" });
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

    public virtual bool OnBlockUpdate(World world, Vector3Int pos) {
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
	// 1 Fl�ssig (textur)   mode: static / dynamic                                      MainAtr height (16)
	// 2 Terrain (textur)           mode: terrain / rounded                            MainAtr grounded (1)
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

	