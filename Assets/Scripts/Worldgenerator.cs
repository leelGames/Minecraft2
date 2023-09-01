using UnityEngine;


public class Worldgen {
	public static int seed;
    public World world;
	static System.Random random;

	public int sealevel = 40;
	public byte oceanBlock = 25;
	public byte underGroundBlock = 1;
	public byte deepUnderGroundBlock = 5;

	int treeID = 1;

    public Worldgen(World world) {
        this.world = world;
		//Zufallsgenerator auf seed einstellen
		if (Main.s.seed == 0) seed = Random.Range(int.MinValue, int.MaxValue);
		else seed = Main.s.seed;
		Debug.Log(seed);
	}

	//Terrainhöhe eines punkts bestimmen
	public TerrainData GenerateTerrain(Vector2Int pos) {
        float max = 0f;
        int maxIndex = 0;
        float finalHeight = 0f;
        float weight;
        float height;

        //Für alle Biome eine gewichtung mit Noise berechnen
        for (int i = 0; i < BiomeData.biomes.Length; i++) {
            weight = Get2Dperlin(pos, 0.02f, i);

            if (weight > max) {
                max = weight;
                maxIndex = i;
            }
            //Terrain höhe ausmitteln (damit keine harten übergänge)
            height = weight * weight * GetTerrainHeight(BiomeData.biomes[i], pos);
            finalHeight += height;


        }
        return new TerrainData(Mathf.FloorToInt(finalHeight), (byte) maxIndex);
    }

    //Terainhöhe durch zwei Layer Noise berechnen
    public float GetTerrainHeight(Biome biome, Vector2Int pos) {
        pos.x /= biome.terrain.stretch;
        return (Get2Dperlin(pos, biome.terrain.scaleL1, -1) * biome.terrain.heightL1 + Get2Dperlin(pos, biome.terrain.scaleL2, -2) * biome.terrain.heightL2);
    }

	public LodgenData GenerateLod(Vector2Int pos) {
		TerrainData data = GenerateTerrain(pos);
		byte color;
		int height = data.terrainheight;
		Biome biome = BiomeData.biomes[data.biomeid];
		float perlin;
		
		if (height < sealevel - 1) {
			height = sealevel - 1;
			color = oceanBlock;
		} else {
			color = (byte)GenerateBlock(new Vector3Int(pos.x, height, pos.y), data);
			if (Get2Dperlin(new Vector2(pos.x, pos.y), biome.type.treeZoneScale, -3) > biome.type.treeZoneThresh) {
				foreach (Tree tree in biome.type.trees) {
					perlin = Get2Dperlin(new Vector2(pos.x, pos.y), 0.8f, tree.stemBlock);
					if (color == tree.groundBlock && perlin > 0.3f) {
						height += Mathf.FloorToInt(tree.maxTreeHeight * (perlin - 0.3f) / (1 - 0.3f));
						color = tree.leafBlock;
					}
				}
			} 
		}
		return new LodgenData(height, color);
	}

    public ushort GenerateBlock(Vector3Int pos, TerrainData data) {
		//Blöcke nach höhe generieren
		Biome biome = BiomeData.biomes[data.biomeid];
		ushort blockID;

		if (pos.y < data.terrainheight - 30) blockID = deepUnderGroundBlock;
		else if (pos.y < data.terrainheight - 15) blockID = underGroundBlock; //stein unten
		else if (pos.y < data.terrainheight - 1) blockID = biome.type.subSurfaceBlock; //erde
		else if (pos.y <= data.terrainheight) blockID = biome.type.surfaceBlock; //gras
		else if (/*biome.isOcean &&*/ pos.y < sealevel) return (ushort)((12 << 11) | oceanBlock);
		else return 0;

		//Erde und Sand Flatschen generieren    Optimeren!!!
		int i = 0;
		foreach (Lode l in biome.type.lodes) {
			if (pos.y > l.minHeight && pos.y < l.maxHeight && Get3Dperlin(pos, l.scale, i) < l.thres) {
				blockID = l.block;
			}
			i++;
		}
		return blockID;
	}

	public byte GenerateStructure(Vector2Int p, TerrainData data) {
		if (data.terrainheight < sealevel) return (byte) treeID;
		Vector3Int pos = new(p.x, data.terrainheight, p.y);
		Biome biome = BiomeData.biomes[data.biomeid];
		Block thisBlock = world.GetVoxel(pos);

		if (Get2Dperlin(new Vector2(pos.x, pos.z), biome.type.treeZoneScale, -3) > biome.type.treeZoneThresh) {
			//Pfanzen (nur stamm)
			foreach (Plant plant in biome.type.plants) {
				if (Get2Dperlin(new Vector2(pos.x, pos.z), plant.plantPlacementScale, plant.block) > plant.plantPlacementThresh) {
					if (thisBlock.id == plant.groundBlock && world.IsGrounded(pos)) {
						int height = (int)(plant.maxHeight * Get2Dperlin(new Vector2(pos.x, pos.z), 5f, plant.groundBlock));
						for (int i = 0; i < height; i++) {
							pos.y++;
							world.SetVoxelFast(pos, plant.block, 0);
						}
					}
				}
			}
			//Bäume
			foreach (Tree tree in biome.type.trees) {
				if (Get2Dperlin(new Vector2(pos.x, pos.z), tree.treePlacementScale, tree.leafBlock) > tree.treePlacementThresh) {
					if (thisBlock.id == tree.groundBlock && world.IsGrounded(pos)) {
						int height = (int)(tree.maxTreeHeight * Get2Dperlin(new Vector2(pos.x, pos.z), 5f, tree.stemBlock) + tree.minTreeHeight);
						GenerateTree(tree, pos, height);
						if (treeID == 0x001F) treeID = 0;
						treeID++;
					}
				}
			}
		}
		//Felsen
		foreach (Rock rock in BiomeData.rocks) {
			float thresh = 0.65f;
			float perlin = Get2Dperlin(new Vector2(pos.x, pos.z), 0.2f, rock.block);
			if (perlin > thresh) {
				if (Get2Dperlin(new Vector2(pos.x, pos.z), rock.rockPlacementScale, rock.maxSize) > rock.rockPlacementThresh) {
					float size = rock.maxSize * (perlin - thresh) / (1 - thresh);
					for (float x = -size; x <= size; x++) {
						for (float y = -size; y <= size; y++) {
							for (float z = -size; z <= size; z++) {
								float dist = Mathf.Sqrt(x * x + y * y + z * z);
								if (dist < size && world.GetVoxel(pos + Vector3Int.RoundToInt(new Vector3(x, y, z))).type == BType.Air) {
									if (Get3Dperlin(pos + new Vector3(x, y, z), 0.5f, rock.block) > dist / size * 0.75f) {
										world.SetVoxelFast(pos + Vector3Int.RoundToInt(new Vector3(x, y, z)), rock.block, 0);
									}
								}
							}
						}
					}
				}
			}
		}
		return (byte) treeID;
	}
	//Rekursives Generierung von Ästen
	void RecursiveTree(Tree tree, Vector3Int pos, int size, int dir) {
		int newdir;
		int leavesize = size / 3 + 2;
		for (int i = size; i > 0; i-= 3) {
			if (Get3DRandom(pos, i) < 0.5f) newdir = dir;
			else if (Get3DRandom(pos, i + size) < 0.5f) newdir = 1;
			else {
				//do { 
					newdir = Range(pos, 2, 5, i - size); 
			//}
				//while (dir + newdir == 5 || dir + newdir == 9);
			}
			if (Get3DRandom(pos, size - i) < (size - i) / (2.5f * size)) { RecursiveTree(tree, pos + VD.dirs[newdir], size / 2, newdir); }
			pos += VD.dirs[newdir];
			world.SetVoxelFast(pos, tree.branchBlock, newdir * 4);
		}
		//Blätter
		for (int x = -leavesize; x <= leavesize; x++) {
			for (int y = -leavesize; y <= leavesize; y++) {
				for (int z = -leavesize; z <= leavesize; z++) {
					float dist = Mathf.Sqrt(x * x + y * y + z * z);
					if (dist < leavesize && world.GetVoxel(pos + new Vector3Int(x, y, z)).type == BType.Air) {
						if (Get3Dperlin(pos + new Vector3(x, y, z), 0.5f, tree.branchBlock) > dist / leavesize * 0.75f) {
							world.SetVoxelFast(pos + new Vector3Int(x, y, z), tree.leafBlock, treeID);
						}
					}
				}
			}
		}
	}

	public void GenerateTree(Tree tree, Vector3Int pos, int height) {
		int dir;
		for (int i = 1; i < height; i++) {
			pos.y++;
		
			if (Get3DRandom(pos, -i) < i / (2f * height)) {	
				dir = Range(pos, 2, 5, - (i + 1));
				RecursiveTree(tree, pos + VD.dirs[dir], i, dir);
				world.SetVoxelFast(pos + VD.dirs[dir], tree.branchBlock, dir * 4);	
			}
			world.SetVoxelFast(pos, tree.stemBlock, 4);
		}
		RecursiveTree(tree, pos, height / 2, 1);
	}

	public static int Range(Vector3Int pos, int min, int max, int offset) {
		return Mathf.FloorToInt(Get3DRandom(pos, offset) * (max - min + 1) + min);
	}
	public static float Range(Vector3Int pos, float min, float max, int offset) {
		return Get3DRandom(pos, offset) * (max - min) + min;
	}
	public static float Get2DRandom(Vector2 pos, int offset) {
		//return (float)rand.NextDouble() % 1;
		//return Get2Dperlin(pos, 10, -1);
		
		return (float)new System.Random((seed + offset).GetHashCode() + pos.GetHashCode()).NextDouble();
	}
	public static float Get3DRandom(Vector3 pos, int offset) {
		//return (float)rand.NextDouble() % 1;
		//return Get3Dperlin(pos, 10, -2);
		
		return (float) new System.Random((seed + offset).GetHashCode() + pos.GetHashCode()).NextDouble();
	}

	public static float Get2Dperlin(Vector2 pos, float scale, int offset) {
		//return Mathf.PerlinNoise(pos.x / VD.ChunkWidth * scale + offset + seedf, pos.y / VD.ChunkWidth * scale + offset + seedf);
		Vector2 o = GetPerlinOffset(offset);
		return Mathf.PerlinNoise((pos.x + o.x) * scale / VD.ChunkWidth, (pos.y + o.y) * scale / VD.ChunkWidth);
	}

	public static float Get2Dperlin(Vector2 pos, float scale, int octaves, float persistance, float lacunarity, int offset) {
		float amp = 1;
		float freq = 1;
		float height = 0;
		for (int i = 0; i < octaves; i++) {
			height += Get2Dperlin(pos, scale * freq, offset + i) * amp;
			amp *= persistance;
			freq *= lacunarity;
		}
		return height;
	}


	public static float Get3Dperlin(Vector3 pos, float scale, int offset) {
		Vector2 o = GetPerlinOffset(offset);
		float x = (pos.x + o.x) * scale;
		float y = (pos.y + o.y) * scale;
		float z = (pos.z + o.x - o.y) * scale;
		//return 0.5f + perlin.Noise(x, y, z);
		float ab = Mathf.PerlinNoise(x, y);
		float bc = Mathf.PerlinNoise(y, z);
		float ac = Mathf.PerlinNoise(x, z);
		float ba = Mathf.PerlinNoise(y, x);
		float cb = Mathf.PerlinNoise(z, y);
		float ca = Mathf.PerlinNoise(z, x);

		return (ab + bc + ac + ba + cb + ca) / 6f;
	}

	static Vector2 GetPerlinOffset(int offset) {
		float x = -Mathf.Lerp(-100000f, 100000f, (seed + offset).GetHashCode() / (float) int.MaxValue);
		float y = Mathf.Lerp(-100000f, 100000f, (seed - offset).GetHashCode() / (float) int.MaxValue);
		//Debug.Log(x);
		return new Vector2(x, y);
	}
}

public class ChunkgenData {
	public int terrainheight;
	public byte biomeid;
	public byte structureid;
	public int minheight;
	public int maxheight;
	public byte lodcolor;

	public TerrainData TerrainData {
		get {
			return new TerrainData(terrainheight, biomeid);
		}
		set { 
			terrainheight = value.terrainheight;
			biomeid = value.biomeid;
		}
	}
	public LodgenData LodData {
		get {
			return new LodgenData(maxheight, lodcolor);
		}
		set {
			maxheight = value.height;
			lodcolor = value.blockcolor;
		}
	}
}

public struct TerrainData {
	public int terrainheight;
	public byte biomeid;

	public TerrainData(int terrainheight, byte biomeid) {
		this.terrainheight = terrainheight;
		this.biomeid = biomeid;
	}
}

public struct LodgenData {
	public int height;
	public byte blockcolor;

	public LodgenData(int height, byte blockcolor) {
		this.height = height;
		this.blockcolor = blockcolor;
	}
}
