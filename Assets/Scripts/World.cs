using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class World : MonoBehaviour {

    public Settings settings;
    public Worldgen gen;
    public WorldThreading threads;
    public Player player;

	public List<BlockEvent> blocksToUpdate;

    public Vector2Int playerChunkCoord;
    public Vector2Int playerLodCoord;
    Vector2Int spawnPoint;

    //Liste aller Chunks
    Dictionary<Vector2Int, Chunk> chunks; 
    Dictionary<Vector2Int, LODHeightMap> lods;
    List<Chunk> activeChunks;
    List<LODHeightMap> activeLods;

    void Awake() {
        Main.Init(settings);
    }
    void Start() {
        threads = new WorldThreading(this);
		gen = new Worldgen(this);
		chunks = new Dictionary<Vector2Int, Chunk>();
        lods = new Dictionary<Vector2Int, LODHeightMap>();
        activeChunks = new List<Chunk>();
        activeLods = new List<LODHeightMap>();
        blocksToUpdate = new();

		//Spieler platzieren
		spawnPoint = new Vector2Int(10 * VD.ChunkWidth / 2, 10 * VD.ChunkWidth / 2);
        player.Position = new Vector3Int(spawnPoint.x, gen.GenerateTerrain(spawnPoint).terrainheight + 2, spawnPoint.y);
        player.highlight.breakPos = player.Position;
        playerChunkCoord = GetChunkCoord(player.Position) + Vector2Int.one;
        playerLodCoord = GetLodCoord(player.Position) + Vector2Int.one; 
 
        //Welt vorgenerieren
        LoadChunks(playerChunkCoord);
        //while (chunksToGenerate.Count > 0 || chunksToUpdate.Count > 0);		
        if (Main.s.farLods) LoadLODs(playerLodCoord);
        Bounds b = new Bounds(Vector3.zero, Vector3.one);
        //Debug.Log(Vector3Int.RoundToInt(b.center) + " " + Vector3Int.RoundToInt(b.size));

    }

    void OnDisable() {
        threads.Stop();
    }

    void FixedUpdate() {
        HandleBlockEvents();

        //neue Chunks laden wenn der Spieler sich bewegt
        if (!(GetChunkCoord(player.Position) + Vector2Int.one).Equals(playerChunkCoord)) {
            playerChunkCoord = GetChunkCoord(player.Position) + Vector2Int.one;
            LoadChunks(playerChunkCoord);
        }
        if (Main.s.farLods && !(GetLodCoord(player.Position) + Vector2Int.one).Equals(playerLodCoord)) {
            playerLodCoord = GetLodCoord(player.Position) + Vector2Int.one; 
			LoadLODs(playerLodCoord);
        }
    }

    void Update() {
        threads.Draw();
    }

    void HandleBlockEvents() { 
        Chunk c;
        for (int i = 0; i < blocksToUpdate.Count; i++) {
            if (blocksToUpdate[i].delay == 0) {
                c = GetChunkP(blocksToUpdate[i].pos);
                if (c.GetBlock(blocksToUpdate[i].pos - c.Position).OnBlockUpdate(c, blocksToUpdate[i].pos - c.Position)) {
                    UpdateBlockFast(blocksToUpdate[i].pos);
                    //threads.chunksToUpdate.AddBuffer(new UpdateEvent(GetChunkCoord(blocksToUpdate[i].pos), 0, true, false));
                }
            }
            blocksToUpdate[i] = blocksToUpdate[i].Tick();
        }
        blocksToUpdate.RemoveAll(e => e.delay == -1);
	}

    void LoadChunks(Vector2Int coord) {
        Chunk c;
		Vector2Int newcoord;
        //Alle Chunks deaktivieren
		foreach (Chunk chunk in activeChunks) {
            if (Main.s.farLods && chunk.state == 5) {
               threads.lodsToUpdate.AddBuffer(new UpdateEvent(GetLodCoord(chunk.Position), 0));
            }
            chunk.Active = false;
		}
		activeChunks.Clear();
		for (int x = coord.x - Main.s.renderDistance - 1; x <= coord.x + Main.s.renderDistance + 1; x++) {
			for (int z = coord.y - Main.s.renderDistance - 1; z <= coord.y + Main.s.renderDistance + 1; z++) {
				newcoord = new Vector2Int(x, z);
				//Chunk erstellen
				if (!chunks.ContainsKey(new Vector2Int(x, z))) {
					c = new GameObject().AddComponent<Chunk>();
					c.Init(newcoord, this);
					chunks.Add(newcoord, c);
					c.Active = false;
				}
			}
		}
		int dist = 0;
		int level;

		while (dist < Main.s.renderDistance) {
			level = Mathf.FloorToInt((float)dist / Main.s.renderDistance * 2);

			for (int x = coord.x - dist; x < coord.x + dist; x++) {
				for (int z = coord.y - dist; z < coord.y + dist; z++) {
					int sx = x - coord.x + 1;
					int sz = z - coord.y + 1;
					if (Mathf.Sqrt(sx * sx + sz * sz) < dist) {
						newcoord = new Vector2Int(x, z);
						c = GetChunkC(newcoord);

						if (!c.Active) { 
                            //neuen Chunk generieren
                            if (c.state == 0) {
							    threads.chunksToGenerate.Add(new UpdateEvent(newcoord, Main.s.closeLods ? level: 0, false, !(Main.s.closeLods && level == 1)));
							    c.state = 1;
						    }
                            //closeLods
                            else if (Main.s.closeLods && c.state >=3 && c.level != level) {
                                if (level == 1 && c.render.lodmesh == null) threads.chunksToUpdate.AddBuffer(new UpdateEvent(newcoord, level, false, false));
                                else if (level == 0 && c.render.mesh == null) threads.chunksToUpdate.AddBuffer(new UpdateEvent(newcoord, level, false, true));
								else c.SetLod(level);
                            }
							//Chunk Laden
							activeChunks.Add(c);
							c.Active = true;
						}
					}
				}
			}
			dist++;
		}
	}
	
    void LoadLODs(Vector2Int coord) {
        LODHeightMap lod;
		Vector2Int newcoord;
        foreach(LODHeightMap alod in activeLods) {
            alod.Active = false;
        }
        activeLods.Clear();
        for (int x = coord.x - Main.s.lodrenderDistance - 1; x < coord.x + Main.s.lodrenderDistance + 1; x++) {
            for (int z = coord.y - Main.s.lodrenderDistance - 1; z < coord.y + Main.s.lodrenderDistance + 1; z++) {
                newcoord = new Vector2Int(x, z);
                
                if (!lods.ContainsKey(new Vector2Int(newcoord.x, newcoord.y))) {
                    lod = new GameObject().AddComponent<LODHeightMap>();
                    lod.Init(newcoord, this);
                    lod.Active = false;
                    lods.Add(newcoord, lod);
                }
            }
        }
        int d = Main.s.renderDistance * VD.ChunkWidth / VD.LODWidth + 2;

		int dist = d;
        int level;
        while (dist < Main.s.lodrenderDistance) {
            level = Mathf.FloorToInt(Mathf.Sqrt((float)(dist - d) / Main.s.lodrenderDistance) * 5);
			
            for (int x = coord.x - dist; x < coord.x + dist; x++) {
                for (int z = coord.y - dist; z < coord.y + dist; z++) {
                    int sx = x - coord.x + 1;
                    int sz = z - coord.y + 1;
                    if (Mathf.Sqrt(sx * sx + sz * sz) < dist) { 
                        newcoord = new Vector2Int(x, z);
                        lod = GetLodC(newcoord);

                        if (!lod.Active) {
                            if (lod.level != level) {
                                threads.lodsToUpdate.AddBuffer(new UpdateEvent(newcoord, level));
                            }
							lod.Active = true;
                            activeLods.Add(lod);
                        }
                    }
                }
            }
            dist++;
        }       
    }
    //Glatte Oberfläche
    public bool IsFlat(Vector3Int pos) {
        return (GetBlock(pos).type != BType.Terrain && GetBounds(pos).max.y == 1f) || IsGrounded(pos);
    }

    // Damit Strukturen auf flachem Terrain generieren
    public bool IsGrounded(Vector3Int pos) {
        if (GetChunkP(pos).state < 4) return GetBlock(pos).type == BType.Terrain && IsTerrainFlat(pos);
		else return GetBlock(pos).type == BType.Terrain && GetVoxelData(pos).mainAtr == 1;
    }
    
    public bool IsTerrainFlat(Vector3Int pos) {
       byte marchIndex = 0;
        for (int i = 0; i < 26; i++) {
            if (GetBlock(pos + VD.dirs[i]).type != BType.Terrain) {
                marchIndex |= VD.tBytes[i];
            }
        }
        for (int i = 0; i < 6; i++) {
            if (marchIndex == VD.tBytes[i]) return true;
        }
        return false;
    }

    //Korrekturen für negative Koordinaten
    public Vector2Int GetChunkCoord(Vector3Int pos) {
        return new Vector2Int(pos.x < 0 ? (pos.x + 1) / VD.ChunkWidth - 1 : pos.x / VD.ChunkWidth, pos.z < 0 ? (pos.z + 1) / VD.ChunkWidth - 1 : pos.z / VD.ChunkWidth);
    }

    public Chunk GetChunkP(Vector3Int pos) {
        return GetChunkC(new Vector2Int(pos.x < 0 ? (pos.x + 1) / VD.ChunkWidth - 1: pos.x / VD.ChunkWidth, pos.z < 0 ? (pos.z + 1) / VD.ChunkWidth - 1 : pos.z / VD.ChunkWidth));
    }

    public Chunk GetChunkC(Vector2Int coord) {
        if (chunks.TryGetValue(coord, out Chunk chunk)) {
            return chunk;
        }
        return null;
    }
	public Vector2Int GetLodCoord(Vector3Int pos) {
        return new Vector2Int(pos.x < 0 ? (pos.x + 1) / VD.LODWidth - 1 : pos.x / VD.LODWidth, pos.z < 0 ? (pos.z + 1) / VD.LODWidth - 1 : pos.z / VD.LODWidth);
	}

	public LODHeightMap GetLodP(Vector3Int pos) {
		return GetLodC(new Vector2Int(pos.x < 0 ? (pos.x + 1) / VD.LODWidth - 1 : pos.x / VD.LODWidth, pos.z < 0 ? (pos.z + 1) / VD.LODWidth - 1 : pos.z / VD.LODWidth));
	}
	public LODHeightMap GetLodC(Vector2Int coord) {
		if (lods.TryGetValue(coord, out LODHeightMap lod)) {
			return lod;
		}
		return null;
	}
    public void UpdateBlockFast(Vector3Int pos) {
		ChunkgenData d = GetGenData(pos);
		if (d.maxheight < pos.y) { d.maxheight = pos.y; d.lodcolor = (byte) GetVoxel(pos); } else if (d.minheight > pos.y) d.minheight = pos.y;
		
        threads.chunksToUpdate.AddBuffer(new UpdateEvent(GetChunkCoord(pos), 0, false, true));
	}

	//Updatet alle Chunks an die der Block angerenzt
	public void UpdateBlock(Vector3Int pos) {
        ChunkgenData d;
   
        d = GetGenData(pos);
        if (d.maxheight < pos.y) { d.maxheight = pos.y; d.lodcolor = (byte) GetVoxel(pos); }
        else if (d.minheight > pos.y) d.minheight = pos.y;

		Vector2Int coord = GetChunkCoord(pos);
		threads.chunksToUpdate.Add(new UpdateEvent(coord, 0, false, true));
        
        Vector3Int newpos;
		for (int i = 2; i < 10; i++) {
			coord = GetChunkCoord(pos + VD.dirs[i]);
			threads.chunksToUpdate.AddBuffer(new UpdateEvent(coord, 0, false, true));
            
            newpos = pos + VD.dirs[i];
            d = GetGenData(newpos);
			if (d.maxheight < newpos.y) { d.maxheight = pos.y; d.lodcolor = (byte) GetVoxel(pos); } 
            if (d.minheight > newpos.y) d.minheight = newpos.y;
		}

        blocksToUpdate.Add(new BlockEvent(pos, 2));
        
        for (int i = 0; i < 6; i++) {
            blocksToUpdate.Add(new BlockEvent(pos + VD.dirs[i], 1));
		}
       
	}

    public bool AddBlockEvent(BlockEvent e) {
        for (int i = 0; i < blocksToUpdate.Count; i++) {
            if (blocksToUpdate[i].pos == e.pos) return false;
        }
        blocksToUpdate.Add(e);
        return true;
    }

    //Befindet sich der block Vertikal im Bereich der Welt
    bool IsVoxelInBounds(Vector3Int pos) {
        return (pos.y >= 0 && pos.y < VD.ChunkHeight);
    }

	public Bounds GetBounds(Vector3Int pos) {
		if (!IsVoxelInBounds(pos)) return new Bounds();
		Chunk chunk = GetChunkP(pos);
        Bounds b = chunk.GetBounds(pos - chunk.Position);
        return b;
	}

    public ChunkgenData GetGenData(Vector3Int pos) {
		if (!IsVoxelInBounds(pos)) return new ChunkgenData();
		Chunk chunk = GetChunkP(pos);
		return chunk.genData[pos.x - chunk.Position.x, pos.z - chunk.Position.z];
	}
    public int GetVoxel(Vector3Int pos) {
		if (!IsVoxelInBounds(pos)) return 0;
		Chunk chunk = GetChunkP(pos);
		return chunk.GetVoxel(pos - chunk.Position);
	}
	public Block GetBlock(Vector3Int pos) {
		if (!IsVoxelInBounds(pos)) return BD.blocks[0];
		Chunk chunk = GetChunkP(pos);
		return chunk.GetBlock(pos - chunk.Position);
	}

	public VoxelData GetVoxelData(Vector3Int pos) {
		if (!IsVoxelInBounds(pos)) return new VoxelData(BD.blocks[0], new byte[0]);
		Chunk chunk = GetChunkP(pos);
		return new VoxelData(chunk.GetBlock(pos - chunk.Position), chunk.GetData(pos - chunk.Position));
	}
	
	public void SetVoxel(Vector3Int pos, int id) {
		if (!IsVoxelInBounds(pos)) return;
		Chunk chunk = GetChunkP(pos);
		chunk.SetVoxel(pos - chunk.Position, id);
	}

	public void SetVoxel(Vector3Int pos, int id, int atr) {
		if (!IsVoxelInBounds(pos)) return;
		Chunk chunk = GetChunkP(pos);
		chunk.SetVoxel(pos - chunk.Position, id, atr);	
	}

	public void SetVoxel(Vector3Int pos, int id, byte[] data) {
		if (!IsVoxelInBounds(pos)) return;
		Chunk chunk = GetChunkP(pos);
		chunk.SetVoxel(pos - chunk.Position, id, data);
	}

    public void CopyVoxel(Vector3Int pos, VoxelData data) {
		if (!IsVoxelInBounds(pos)) return;
		Chunk chunk = GetChunkP(pos);
		chunk.CopyVoxel(pos - chunk.Position, data);
	}

    public void CreateFallingStructure(Vector3Int pos) {
        Vector3Int[] a;
        if (GetBlock(pos).type >= BType.Custom) {
            a = Traverse(pos).ToArray();
            if (a.Length > 0) {
                VoxelEntity ve = VoxelEntity.Create(this, a);
                Clear(a);
                ve.Active = true;
            }
        }
    }
    //List<Vector3Int> TraverseFor(Vector3Int pos, (VoxelData a, VoxelData b) compare -> bool) {return null;}
    List<Vector3Int> Traverse(Vector3Int pos) {
        List<Vector3Int> list = new();
        Queue<Vector3Int> queue = new();
        Chunk chunk;
        Vector3Int next;
        bool cancel = false; 
        queue.Enqueue(pos);
        list.Add(pos);

        while (queue.Count > 0) {
            pos = queue.Dequeue();
         
            for (int i = 0; i < 6; i++) {
                next = pos + VD.dirs[i];
                if ((GetBlock(pos).isSolid && IsGrounded(next)) || queue.Count > 10000) {
                    cancel = true;
                    queue.Clear();
                    break;
                }
                chunk = GetChunkP(next);
				if (!chunk.GetFlag(next - chunk.Position) && Connects(pos, next, i)) {
					chunk.SetFlag(next - chunk.Position, true);
					queue.Enqueue(next);
                    list.Add(next);
                }
            }
            //list.Add(pos);
        }
        for (int i = 0; i < list.Count; i++) {
            chunk = GetChunkP(list[i]);
            chunk.SetFlag(list[i] - chunk.Position, false);
		}
        if (cancel) list.Clear();
        return list;
    }

    bool Connects(Vector3Int a, Vector3Int b, int dir) {
        Block blockA = GetBlock(a);
        Block blockB = GetBlock(b);
        
        if (blockB.type <= BType.Terrain) return false;


        //Extra Regel für Bäume
        if (blockB.id == 10) {
            return blockA.id != blockB.id || GetVoxelData(a).mainAtr == GetVoxelData(b).mainAtr;
        } 
        //Connect, wenn Bounds sich berühren
        Bounds ba = GetBounds(a);
        Bounds bb = GetBounds(b);

        return dir switch {
		   0 => ba.min.y <= 0f && bb.max.y >= 1f,
		   1 => ba.max.y >= 1f && bb.min.y <= 0f,
		   2 => ba.min.x <= 0f && bb.max.x >= 1f,
		   3 => ba.max.x >= 1f && bb.min.x <= 0f,
		   4 => ba.min.z <= 0f && bb.max.z >= 1f,
		   5 => ba.max.z >= 1f && bb.min.z <= 0f,
		   _ => false,
			/* 0 => Mathf.RoundToInt(ba.min.y * 10) / 10 == 0 && Mathf.RoundToInt(bb.max.y * 10) / 10 == 1,
			 1 => Mathf.RoundToInt(ba.max.y * 10) / 10 == 1 && Mathf.RoundToInt(bb.min.y * 10) / 10 == 0,
			 2 => Mathf.RoundToInt(ba.min.x * 10) / 10 == 0 && Mathf.RoundToInt(bb.max.x * 10) / 10 == 1,
			 3 => Mathf.RoundToInt(ba.max.x * 10) / 10 == 1 && Mathf.RoundToInt(bb.min.x * 10) / 10 == 0,
			 4 => Mathf.RoundToInt(ba.min.z * 10) / 10 == 0 && Mathf.RoundToInt(bb.max.z * 10) / 10 == 1,
			 5 => Mathf.RoundToInt(ba.max.z * 10) / 10 == 1 && Mathf.RoundToInt(bb.min.z * 10) / 10 == 0,
			 _ => false,*/
		};

        //return !VoxelRenderer.DrawVoxelFace(GetBounds(b), GetBounds(a), dir);

        /*switch (a.block.type) {
            case 0: return false;
            case 1: return b.block.id == 22;
            case 2: return false;
            case 3: return !(b.block.type != 3 || (b.mainAtr != 0 && b.mainAtr != a.mainAtr) || VD.slabData[a.mainAtr, dir] != 0f || (VD.slabData[b.mainAtr, (dir % 2 == 0 ? dir + 1 : dir - 1)] != 0f));
            case 4: return b.block.type == 3 || b.block.type == 4;
            case 5: return b.block.type == 5;
            case 6: return b.block.type == 6 && (a.mainAtr == dir / 2 || b.mainAtr == dir / 2) || b.block.id == 22;
			case 7: return false;
            default: return false;
		}
        //return ba.type == bb.type;
        switch (bb.type) {
            case BType.Air: return false;
            case BType.Liquid: return false;
            case BType.Terrain: return false;
            case BType.Custom: return ba.connectMode == CMode.Pipe || (ba.id == 22 && bb.id == 22 && GetVoxelData(a).mainAtr == GetVoxelData(b).mainAtr);
            case BType.CustomSlab: return false;
            case BType.Voxel: return ba.type == BType.Voxel; 
            default: return false;
        }*/
    }

	public void Clear(Vector3Int[] voxels) {
		for (int i = 0; i < voxels.Length; i++) {
			SetVoxel(voxels[i], 0);
            UpdateBlockFast(voxels[i]);
            //threads.chunksToUpdate.AddBuffer(new UpdateEvent(GetChunkCoord(voxels[i]), 0, false, true));
		}
	}

    public void ClearArea(Vector3Int pos1, Vector3Int pos2) {
        Vector3Int width = pos2 - pos1;
        for (int x = 0; x <= width.x; x++) {
            for (int z = 0; z <= width.z; z++) {
                for (int y = 0; y <= width.y; y++) {
                    SetVoxel(pos1 + new Vector3Int(x, y, z), 0);
                }
            }
        }
    }
    //Nocj mache3n
    public void GrowTree(Vector3Int pos, int size) {
        SetVoxel(pos, 0);
        Clear(Traverse(pos + Vector3Int.up).ToArray());
        gen.GenerateTree(BiomeData.trees[0], pos, size);
        UpdateBlockFast(pos);
    }
}

public struct VoxelData {
    public Block block;
    //public bool flag;
    //public int mainAtr;
    public byte[] data;
    public int mainAtr {
        get {return data.Length != 0 ? data[0] : 0;}
    }

    public VoxelData(Block block, byte[] data) {
      
        this.block = block;
        this.data = data;
    }
	/*public VoxelData(Block block, int mainAtr) {
		
		this.block = block;
        this.data = new byte[0];
	}*/
}

