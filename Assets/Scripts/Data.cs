using UnityEngine;

public static class VD {
    public static readonly int ChunkWidth = 16;
	public static readonly int LODWidth = 240;
	public static readonly int subChunks = 16;
	
	public static int ChunkHeight {
		get { return subChunks * ChunkWidth; }
	}

	//Vereinfachung der Higligtcodes f�r slabs mit vorgerechneten vectoren
	public static readonly float third = 1.0f / 3.0f;
	public static readonly Vector3[,,] highlightVectors = new Vector3[2, 3, 3] {
		{
			{ new Vector3(1f, 0.5f, 1f), new Vector3(1f, 2f, 1f), new Vector3(0.5f, 0.25f, 0.5f) },
			{ new Vector3(0.5f, 1f, 1f), new Vector3(2f, 1f, 1f), new Vector3(0.25f, 0.5f, 0.5f) },
			{ new Vector3(1f, 1f, 0.5f), new Vector3(1f, 1f, 2f), new Vector3(0.5f, 0.5f, 0.25f) },
		}, {
			{ new Vector3(1f, third, 1f), new Vector3(1f, 3f, 1f), new Vector3(0.5f, 0.5f - third, 0.5f) },
			{ new Vector3(third, 1f, 1f), new Vector3(3f, 1f, 1f), new Vector3(0.5f - third, 0.5f, 0.5f) },
			{ new Vector3(1f, 1f, third), new Vector3(1f, 1f, 3f), new Vector3(0.5f, 0.5f, 0.5f - third) },
		}
	};

	//allgemeine Reihenfolge:  unten(o), oben(u), links(l), rechts(r), vorne(v), hinten(h)

	//Speichert Bounds aller m�glichen Slabs
	public static readonly Bounds[] slabBounds = new Bounds[] {
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Null

		new Bounds(new Vector3(0.5f, 0.25f, 0.5f), new Vector3(1f, 0.5f, 1f)), //Bottomhalfslab 0
		new Bounds(new Vector3(0.5f, 0.75f, 0.5f), new Vector3(1f, 0.5f, 1f)), //Tophalfslab 1
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 2
		new Bounds(new Vector3(0.25f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f)), //Lefthalfslab 3
		new Bounds(new Vector3(0.75f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f)), //Righthalfslab 4
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 5
		new Bounds(new Vector3(0.5f, 0.5f, 0.25f), new Vector3(1f, 1f, 0.5f)), //Fronthalfslab 7
		new Bounds(new Vector3(0.5f, 0.5f, 0.75f), new Vector3(1f, 1f, 0.5f)), //Backhalfslab 6
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 8
	
		new Bounds(new Vector3(0.5f, 0.16667f, 0.5f), new Vector3(1f, 0.33333f, 1f)), //Bottomthirdslab 0
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 0.33333f, 1f)), //Middlethirdslab 1
		new Bounds(new Vector3(0.5f, 0.83333f, 0.5f), new Vector3(1f, 0.33333f, 1f)), //Topthirdslab 2
		new Bounds(new Vector3(0.5f, 0.33333f, 0.5f), new Vector3(1f, 0.66666f, 1f)), //BMthirdslab 3
		new Bounds(new Vector3(0.5f, 0.66666f, 0.5f), new Vector3(1f, 0.66666f, 1f)),
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 5
		new Bounds(new Vector3(0.16667f, 0.5f, 0.5f), new Vector3(0.33333f, 1f, 1f)), //Leftthirdslab 6
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.33333f, 1f, 1f)), //Middlethirdslab 7
		new Bounds(new Vector3(0.83333f, 0.5f, 0.5f), new Vector3(0.33333f, 1f, 1f)), //Rightthirdslab 8
		new Bounds(new Vector3(0.33333f, 0.5f, 0.5f), new Vector3(0.66666f, 1f, 1f)), //LMthirdslab 9
		new Bounds(new Vector3(0.66666f, 0.5f, 0.5f), new Vector3(0.66666f, 1f, 1f)), //MRthirdslab 10
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 11
		new Bounds(new Vector3(0.5f, 0.5f, 0.16667f), new Vector3(1f, 1f, 0.33333f)), //Frontthirdslab 14
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 0.33333f)), //Middlethirdslab 13
		new Bounds(new Vector3(0.5f, 0.5f, 0.83333f), new Vector3(1f, 1f, 0.33333f)), //Backthirdslab 12
		new Bounds(new Vector3(0.5f, 0.5f, 0.33333f), new Vector3(1f, 1f, 0.66666f)), //FMtthirdslab 16
		new Bounds(new Vector3(0.5f, 0.5f, 0.66666f), new Vector3(1f, 1f, 0.66666f)), //BMthirdslab 15
		new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)), //Full 17
	};

	//Speichert Typ-ID der Bl�cke in Slabdata
	public static int[] slabDataToID = new int[] {-1, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5};

	//Definiert welcher Slab-Typ platziert wird abh�ngt von aktuellem Slabtyp (x) und highlight Position (y)
	public static int[,] halfSlabCombiner = new int[4, 4] {
		//place
		//leer unten oben voll
		{ 0, -1, 2, -1}, //unten
		{ 1, 2, -1, -1}, //oben
		//break
		{ -1, -1, 1, 1}, //unten
		{ -1, 0, -1, 0}, //oben

	};
	public static int[,] thirdSlabCombiner = new int[6, 7] {
		//place
		//leer unten mitte oben unten-mitte oben-mitte voll
		{ 0, -1, 3, -1, -1, 5, -1 }, //unten
		{ 1, 3, -1, 4, -1, -1, -1 }, //mitte
		{ 2, -1, 4, -1, 5, -1, -1 }, //oben
		//break
		{ -1, -1, 1, 2, 1, 4, 4 }, //unten
		{ -1, 0, -1, 2, 0, 2, 5 }, //mitte
		{ -1, 0, 1, -1, 3, 1, 3 }, //oben
	};

	//Definert die Position der Ecken eines W�rfels
	public static readonly Vector3Int[] voxelVerts = new Vector3Int[8] {
		new Vector3Int(0,0,0),
		new Vector3Int(1,0,0),
		new Vector3Int(1,1,0),
		new Vector3Int(0,1,0),
		new Vector3Int(0,0,1),
		new Vector3Int(1,0,1),
		new Vector3Int(1,1,1),
		new Vector3Int(0,1,1)
	};

	//Speichert Rotierungen der VoxelVerts
	public static readonly int[,] vertRots = new int[12, 8] {
		{ 1, 5, 6, 2, 0, 4, 7, 3},
		{ 4, 0, 3, 7, 5, 1, 2, 6},
		{ 0, 1, 2, 3, 4, 5, 6, 7},
		{ 5, 4, 7, 6, 1, 0, 3, 2},

		{ 6, 2, 1, 5, 7, 3, 0, 4},
		{ 3, 7, 4, 0, 2, 6, 5, 1},
		{ 2, 3, 0, 1, 6, 7, 4, 5},
		{ 7, 6, 5, 4, 3, 2, 1, 0},

		{ 4, 7, 6, 5, 0, 3, 2, 1},
		{ 3, 0, 1, 2, 7, 4, 5, 6},
		{ 6, 5, 4, 7, 2, 1, 0, 3},
		{ 1, 2, 3, 0, 5, 6, 7, 4},
	};
	//Rechnet 90Grad RotierungsID in Euler Rotation um (unzuverl�ssig)
	// 0 oben(O U R L)  4 unten(O U R L) 8 rechts(O U R L) 12 links(O U R L) 16 hinten(O U R L) 20 vorne(O U R L) 

	public static readonly Vector3Int[] dirToRot2 = new Vector3Int[24] {
		new Vector3Int(0, 0, 0),//oo	0
		new Vector3Int(0, 180, 0),//ou
		new Vector3Int(0, 90, 0),//or
		new Vector3Int(0, 270, 0),//ol
		
		new Vector3Int(180, 0, 0),//uo		4
		new Vector3Int(180, 180, 0),//uu
		new Vector3Int(180, 90, 0),//ur
		new Vector3Int(180, 270, 0),//ul
		
		new Vector3Int(0, 0, 90),//ro		8
		new Vector3Int(0, 180, 90),//ru
		new Vector3Int(0, 90, 90),//rr
		new Vector3Int(0, 270, 90),//rl
		
		new Vector3Int(0, 0, 270),//lo		12
		new Vector3Int(0, 180, 270),//lu
		new Vector3Int(0, 90, 270),//lr
		new Vector3Int(0, 270, 270),//ll
		
		new Vector3Int(90, 0, 0),//vo		16
		new Vector3Int(90, 180, 0),//vu
		new Vector3Int(90, 90, 0),//vr
		new Vector3Int(90, 270, 0),//vl
		
		new Vector3Int(270, 0, 0),//ho		20	
		new Vector3Int(270, 180, 0),//hu
		new Vector3Int(270, 90, 0),//hr
		new Vector3Int(270, 270, 0),//hl
	};
	//Von Chat GPT
	public static readonly Vector3[] dirToRot3 = new Vector3[24] {
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 90f),
		new Vector3(0f, 0f, 180f),
		new Vector3(0f, 0f, 270f),
		new Vector3(0f, 90f, 0f),
		new Vector3(0f, 90f, 90f),
		new Vector3(0f, 90f, 180f),
		new Vector3(0f, 90f, 270f),
		new Vector3(0f, 180f, 0f),
		new Vector3(0f, 180f, 90f),
		new Vector3(0f, 180f, 180f),
		new Vector3(0f, 180f, 270f),
		new Vector3(0f, 270f, 0f),
		new Vector3(0f, 270f, 90f),
		new Vector3(0f, 270f, 180f),
		new Vector3(0f, 270f, 270f),
		new Vector3(90f, 0f, 0f),
		new Vector3(90f, 0f, 90f),
		new Vector3(90f, 0f, 180f),
		new Vector3(90f, 0f, 270f),
		new Vector3(90f, 90f, 0f),
		new Vector3(90f, 180f, 0f),
		new Vector3(270f, 0f, 0f),
		new Vector3(270f, 0f, 90f),
	};

	//Definiert welche Vertices zu welchem Face geh�ren (f�r verschiebung von Faces)
	public static readonly int[,] voxelFaces = new int[6, 8] {
		{ 0, 0, -1, -1, 0, 0, -1, -1}, //u
		{ -1, -1, 0, 0, -1, -1, 0, 0}, //o
		{ 0, -1, -1, 0, 0, -1, -1, 0}, //l
		{ -1, 0, 0, -1, -1, 0, 0, -1}, //r
		{ 0, 0, 0, 0, -1, -1, -1, -1}, //v
		{ -1, -1, -1, -1, 0, 0, 0, 0}, //h
		
	};


    //Definert in welcher Reihenfolge die Ecken verbunden werden m�ssen um eine quadratische Fl�che aus zwei Dreiecken zu erzeugen
    public static readonly int[] vertexTris = new int[6] {0, 1, 2, 2, 1, 3};
    
    public static readonly int[,] voxelTris = new int[6, 4] {
		{1, 5, 0, 4}, //u
		{3, 7, 2, 6}, //o
		{4, 7, 0, 3}, //l
        {1, 2, 5, 6}, //r
		{0, 3, 1, 2}, //v
		{5, 6, 4, 7}, //h
    };
	
	//Definiert Slope Mesh abh�ngig von VoxelVerts
	public static readonly int[] slopeTris = new int[] { 1, 5, 0, 0, 5, 4,  0, 3, 1, 1, 3, 2,  1, 2, 5,  4, 3, 0,  2, 3, 5, 5, 3, 4};
	public static readonly int[] slopeUVs = new int[]  { 0, 1, 2, 2, 1, 3,  0, 1, 2, 2, 1, 3,  0, 1, 2,  0, 3, 2,  2, 0, 3, 3, 0, 1};

	//Rotierung von Slopes
	public static readonly int[] slopeDirs = new int[24] {
		//Tabelle sehr aufw�ndig nicht anfassen!!
		6, 7, 4, 5,
		2, 3, 0, 1,
		11, 10, 4, 0,
		9, 8, 5, 1,
		2, 6, 11, 9,
		3, 7, 10, 8,
	};

	//F�r berechning von dir12
	public static readonly Vector2[] dir12Table = new Vector2[] {
		new Vector2(0.5f, 0f),
		new Vector2(0.5f, 1f),
		new Vector2(1f, 0.5f),
		new Vector2(0f, 0.5f),
	};

	//Definert wo ein Benachbarter Chunk angrenzt
	public static readonly Vector2Int[] chunkChecks = new Vector2Int[8] {
		new Vector2Int(0, -1),
		new Vector2Int(-1, -1),
		new Vector2Int(-1, 0),
		new Vector2Int(-1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(1, -1)
	};

	//definiert UV Grenzen
	public static readonly Vector2Int[] voxelUvs = new Vector2Int[4] {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
    };

	//F�r verbindendes Wasser
	public static readonly int[,] waterHeight = new int[8,8] {
		{ 0,0,1,1,0,0,0,0 },
		{ 0,0,0,1,0,0,0,0 },
		{ 0,0,0,1,0,0,0,1 },
		{ 0,0,0,0,0,0,0,1 },
		{ 0,0,0,0,0,0,1,1 },
		{ 0,0,0,0,0,0,1,0 },
		{ 0,0,1,0,0,0,1,0 },
		{ 0,0,1,0,0,0,0,0 },

	};

	//Definiert alle Richtungen wo ein benachbarter Block angrenzt
	public static readonly Vector3Int[] dirs = new Vector3Int[26] {
		//Fl�chen
		new Vector3Int(0,-1, 0),//u
		new Vector3Int(0, 1, 0),//o
		new Vector3Int(-1,0, 0),//l //Y0 Zone Start: 2
		new Vector3Int(1, 0, 0),//r
		new Vector3Int(0, 0,-1),//v
		new Vector3Int(0, 0, 1),//h
		//Kanten
		new Vector3Int(-1,0,-1),
		new Vector3Int(1,0,-1),
		new Vector3Int(-1,0,1),
		new Vector3Int(1,0,1), //Y0 Zome End: 10
		new Vector3Int(0,-1,-1),
		new Vector3Int(-1,-1,0),
		new Vector3Int(0,1,-1),
		new Vector3Int(1,-1,0),
		new Vector3Int(0,-1,1),
		new Vector3Int(-1,1,0),
		new Vector3Int(0,1,1),
		new Vector3Int(1,1,0),
		//Ecken
		new Vector3Int(-1,-1,-1),
		new Vector3Int(1,-1,-1),
		new Vector3Int(1,1,-1),
		new Vector3Int(-1,1,-1),
		new Vector3Int(-1,-1,1),
		new Vector3Int(1,-1,1),
		new Vector3Int(1,1,1),
		new Vector3Int(-1,1,1)
	};

	//Definiert die entsprechende ID in der Triangulationstabelle
	public static readonly byte[] tBytes = new byte[26] {
		0b00110011,
		0b11001100,
		0b10011001,
		0b01100110,
		0b00001111,
		0b11110000,
		
		0b00001001,
		0b00000110,
		0b10010000,
		0b01100000,
		0b00000011,
		0b00010001,
		0b00001100,
		0b00100010,
		0b00110000,
		0b10001000,
		0b11000000,
		0b01000100,

		0b00000001,
		0b00000010,
		0b00000100,
		0b00001000,
		0b00010000,
		0b00100000,
		0b01000000,
		0b10000000
	};

	//Definiert welche Kanten ausgehend von der Triangulationstabelle gezeichnet werden 
	public static readonly int[,] tEdges = new int[12, 2] {
		{0,1},{1,2},{3,2},{0,3},{4,5},{5,6},{7,6},{4,7},{0,4},{1,5},{2,6},{3,7}
	};

	//Triangulationstabelle f�r alle m�glichen Terrain Meshes
	public static readonly sbyte[,] tTriangles = new sbyte[256, 16] {
		{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
		{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
		{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
		{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
		{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
		{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
		{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
		{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
		{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
		{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
		{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
		{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
		{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
		{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
		{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
		{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
		{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
		{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
		{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
		{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
		{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
		{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
		{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
		{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
		{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
		{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
		{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
		{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
		{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
		{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
		{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
		{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
		{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
		{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
		{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
		{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
		{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
		{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
		{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
		{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
		{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
		{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
		{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
		{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
		{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
		{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
		{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
		{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
		{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
		{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
		{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
		{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
		{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
		{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
		{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
		{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
		{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
		{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
		{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
		{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
		{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
		{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
		{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
		{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
		{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
		{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
		{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
		{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
		{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
		{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
		{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
		{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
		{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
		{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
		{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
		{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
		{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
		{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
		{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
		{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
		{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
		{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
		{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
		{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
		{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
		{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
		{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
		{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
		{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
		{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
		{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
		{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
		{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
		{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
		{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
		{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
		{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
		{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
		{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
		{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
		{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
		{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
		{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
		{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
		{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
		{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
		{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
		{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
		{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
		{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
		{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
		{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
		{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
		{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
		{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
		{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
		{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
		{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
		{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
		{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
		{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
		{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
		{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
		{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
		{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
		{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
		{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
		{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
		{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
		{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
		{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
		{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
		{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
		{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
		{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
		{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
		{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
		{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
		{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
		{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
		{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
		{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
		{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
		{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
		{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
		{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
		{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
		{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
		{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
		{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
		{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
		{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
		{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
		{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
		{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
		{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
	};
}
