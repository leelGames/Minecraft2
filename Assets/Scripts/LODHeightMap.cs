using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LODHeightMap : MonoBehaviour {
	World world;
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	Texture2D texture;

	Color32[] colorMap;
	Vector2Int coords;
	int relwidth;

	Vector3[] vertices;
	int[] tris;
	Vector2[] uvs;

	public int level;

	private bool isActive = false;
	public bool Active {
		get { return isActive; }
		set {
			isActive = value;
			gameObject.SetActive(value);
		}
	}

	public Vector3Int Position {
		get { return new Vector3Int(coords.x * VD.LODWidth, 0, coords.y * VD.LODWidth); }
	}

	public void Init(Vector2Int coord, World w) {
		this.world = w;
		this.coords = coord;
		level = -1;
	
		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = Main.materials[6];

		gameObject.transform.SetParent(world.transform);
		gameObject.transform.position = Position;
		gameObject.name = "LODMap " + coord.x + ", " + coord.y;
	}

	public void UpdateMesh(int l) {
		if (l != -1) level = l;
		
		int inc = Mathf.RoundToInt(Mathf.Pow(2, level));
		relwidth = VD.LODWidth / inc + 1;

		colorMap = new Color32[(relwidth - 1) * (relwidth - 1)];
		vertices = new Vector3[relwidth * relwidth];
		uvs = new Vector2[relwidth * relwidth];
		tris = new int[(relwidth - 1) * (relwidth - 1) * 6];

		int triIndex = 0;
		int vertIndex = 0;
		
		LodgenData data;
		Chunk chunk;
		for (int z = 0; z <= VD.LODWidth; z += inc) {
			for(int x = 0; x <= VD.LODWidth; x += inc) {
				chunk = world.GetChunkP(new Vector3Int(x + Position.x, 0, z + Position.z));
				
				if (chunk != null && chunk.state >= 3) {
					data = chunk.genData[x % VD.ChunkWidth, z % VD.ChunkWidth].LodData;
				}
				else data = world.gen.GenerateLod(new Vector2Int(x + Position.x, z + Position.z));

				vertices[vertIndex] = new Vector3(x + 0.5f, data.height, z + 0.5f);
				uvs[vertIndex] = new Vector2((float)(x + 0.5f)/ (VD.LODWidth), (float)(z + 0.5f)/ (VD.LODWidth));

				if (x < VD.LODWidth  && z < VD.LODWidth) {
					tris[triIndex] = vertIndex;
					tris[triIndex + 1] = vertIndex + relwidth;
					tris[triIndex + 2] = vertIndex + relwidth + 1;
					tris[triIndex + 3] = vertIndex + relwidth + 1;
					tris[triIndex + 4] = vertIndex + 1;
					tris[triIndex + 5] = vertIndex;
					
					if (chunk == null || !chunk.Active) {
						colorMap[triIndex / 6] = BD.blocks[data.blockcolor].color;
					} else { colorMap[triIndex / 6] = new Color32(0, 0, 0, 0); }
					triIndex += 6;
				}
				vertIndex++;
			}
		}
	}

	public void DrawMesh() {
		texture = new Texture2D((relwidth - 1), (relwidth - 1));
		if (level < 3) texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels32(colorMap);
		texture.Apply();

		Mesh mesh = new();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = tris;
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
		meshRenderer.material.mainTexture = texture;
	}
}

