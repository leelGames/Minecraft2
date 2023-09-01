using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelRenderer {
	readonly AChunk chunk;
	public Mesh mesh;
	public Mesh lodmesh;
	public Mesh collider;

	readonly List<Vector3> vertices;
	readonly List<int> terrainTris;
	readonly List<int> transparentTris;
	readonly List<int> uvTris;
	readonly List<int> waterTris;
	readonly List<Vector3> solidverts;
	readonly List<int> solidTris;
	readonly List<Vector2> uvs;
	readonly List<Vector2> uvs2;

	protected int vertCount;
	protected int solidvertCount;
	
	public VoxelRenderer(AChunk chunk) {
		this.chunk = chunk;
		vertices = new();
		uvs = new();
		terrainTris = new();
		transparentTris = new();
		uvTris = new();
		waterTris = new();
		solidverts = new();
		solidTris = new();
		uvs2 = new();
	}

	public void UpdateMesh(UpdateEvent e, Vector3Int pos) {
		if (e.lod == 0) {
			switch (chunk.GetVoxel(pos).type) {
				case BType.Air: return;
				case BType.Liquid: GetMeshLiquid(pos, e); return;
				case BType.Terrain: GetMeshTerrain(pos, e); return;
				case BType.Rounded: GetMeshRounded(pos, e); return;
				case BType.Custom: GetMeshCustom(pos, e); return;
				case BType.Voxel: GetMeshVoxel(pos, e); return;
				case BType.CustomSlab: GetMeshCustomSlab(pos, e); return;
			}
		} else {
			switch (chunk.GetVoxel(pos).type) {
				case BType.Air: return;
				case BType.Liquid: GetMeshBounds(pos, e); return;
				case BType.Terrain: GetMeshTerrain(pos, e); return;
				case BType.Rounded: GetMeshRounded(pos, e); return;
				case BType.Custom: GetMeshBounds(pos, e); return;
				case BType.Voxel: GetMeshVoxel(pos, e); return;
				case BType.CustomSlab: GetMeshBounds(pos, e); return;
			}
		}
	}

	public void DrawMesh(UpdateEvent e) {
		if (e.lod == 0) {
			chunk.meshRenderer.materials = new Material[] { Main.materials[0], Main.materials[1], Main.materials[2], Main.materials[3] };
			mesh = new();
			if (vertCount > 65535) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			mesh.vertices = vertices.ToArray();
			mesh.subMeshCount = 4;
			mesh.SetTriangles(terrainTris.ToArray(), 0);
			mesh.SetTriangles(transparentTris.ToArray(), 1);
			mesh.SetTriangles(uvTris.ToArray(), 2);
			mesh.SetTriangles(waterTris.ToArray(), 3);
			mesh.uv = uvs.ToArray();
			mesh.uv2 = uvs2.ToArray();
			mesh.RecalculateNormals();
			chunk.meshFilter.mesh = mesh;
		}
		else {
			chunk.meshRenderer.materials = new Material[] { Main.materials[4], Main.materials[5]};
			lodmesh = new();
			if (vertCount > 65535) lodmesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			lodmesh.vertices = vertices.ToArray();
			lodmesh.uv = uvs.ToArray();
			lodmesh.subMeshCount = 2;
			lodmesh.SetTriangles(terrainTris.ToArray(), 0);
			lodmesh.SetTriangles(transparentTris.ToArray(), 1);

			lodmesh.RecalculateNormals();
			chunk.meshFilter.mesh = lodmesh;
			//chunk.meshCollider.sharedMesh = null;
		}
		if (e.collider) {
			collider = new();
			if (solidvertCount > 65535) collider.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //Fix für verbugtes Custom 
			collider.vertices = solidverts.ToArray();
			collider.triangles = solidTris.ToArray();
			chunk.meshCollider.sharedMesh = collider;
		}
		vertices.Clear();
		terrainTris.Clear();
		transparentTris.Clear();
		uvTris.Clear();
		solidverts.Clear();
		solidTris.Clear();
		waterTris.Clear();
		uvs.Clear();
		uvs2.Clear();
		vertCount = 0;
		solidvertCount = 0;
	}

	void AddVert(Vector3 vert, Vector2 uv, UpdateEvent e, Block block) {
		if (e.lod == 0) {
			vertices.Add(vert);
			uvs.Add(new Vector2(block.textureID, block.textureScale));

			if (block.shaderType < SMode.Triplanar) uvs2.Add(uv);
			else uvs2.Add(Vector2.zero);
		} else {
			vertices.Add(vert);
			uvs.Add(new Vector2(0, (block.textureID) / 64f));
		}
		vertCount++;

		if (e.collider && block.isSolid) {
			solidverts.Add(vert);
			solidvertCount++;
		}
	}

	void AddTri(int tri, int offset, UpdateEvent e, Block block) {
		int value = tri + vertCount - offset;
		if (e.lod == 0) {
			if (block.shaderType == SMode.Triplanar) terrainTris.Add(value);
			else if (block.shaderType == SMode.UVCutoff) uvTris.Add(value);
			else if (block.shaderType == SMode.UVAlpha) transparentTris.Add(value);
			else if (block.shaderType == SMode.Water) waterTris.Add(value);
		} else {
			if (block.shaderType == SMode.UVAlpha) transparentTris.Add(value);
			else terrainTris.Add(value);
		}

		if (e.collider && block.isSolid) {
			solidTris.Add(tri + solidvertCount - offset);
		}
	}

	public int GetMeshIndex(Vector3Int pos) {
		Block thisBlock = chunk.GetVoxel(pos);
		int thisAtr = chunk.GetVoxelAtr(pos);

		//Fügt gespeicherte Ecken und Dreiecke hinzu
		int meshIndex = 0;
		if (thisBlock.connectMode != CMode.None) {

			if (thisBlock.connectMode == CMode.Grid) {
				//ermittelt id anhand benachbarter grid Blöcke
				for (int i = 0; i < 6; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).id == thisBlock.id) {
						meshIndex |= 1 << i;
					}
				}
				if (meshIndex == 0b111111) return 0;
			} else if (thisBlock.connectMode == CMode.Pipe) {
				//ermittelt id anhand benachbarter pipe Blöcke und richtung
				thisAtr /= 8;
				for (int i = 0; i < 6; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).id == thisBlock.id && (thisAtr == i / 2 || chunk.CheckBlockAtr(pos + VD.dirs[i]) / 8 == i / 2)) {
						meshIndex |= 1 << i;
					}
				}
				meshIndex += thisAtr * 64;
			} else if (thisBlock.connectMode == CMode.Horizontal) {
				for (int i = 0; i < 4; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i + 2]).id == thisBlock.id) {
						meshIndex |= 1 << i;
					}
				}
			}
		 	else if (thisBlock.connectMode == CMode.Vertical) {
				for (int i = 0; i < 4; i++) {
					if (chunk.CheckBlock(pos + VD.dirs[i]).id == thisBlock.id) {
						meshIndex |= 1 << i;
					}
				}
				if (meshIndex == 15) return 0;
			}
		}
		return meshIndex;
	}

	protected void GetMeshCustom(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		int thisAtr = chunk.GetVoxelAtr(pos);
		Bounds b = chunk.GetBounds(pos);

		//umschlossene Blöcke werden nicht geupdated
		int check = 0;
		for (int i = 0; i < 6; i++) {
			if (chunk.CheckBlock(pos + VD.dirs[i]).type < BType.Terrain) check++;
		}
		if (check == 0) return;
		int meshIndex = GetMeshIndex(pos);

		if (thisBlock.connectMode != CMode.None || thisBlock.rotMode == RMode.None) {
			for (int i = 0; i < Main.meshTable[thisBlock.meshID][meshIndex].vertices.Length; i++) {
				AddVert(Main.meshTable[thisBlock.meshID][meshIndex].vertices[i] + pos, Main.meshTable[thisBlock.meshID][meshIndex].uv[i], e, thisBlock);
			}
		} else {
			for (int i = 0; i < Main.meshTable[thisBlock.meshID][meshIndex].vertices.Length; i++) {
				AddVert(RotateVert(Main.meshTable[thisBlock.meshID][meshIndex].vertices[i], thisAtr) + pos - new Vector3(0, b.min.y, 0), Main.meshTable[thisBlock.meshID][meshIndex].uv[i], e, thisBlock);
			}
		}
		for (int j = 0; j < Main.meshTable[thisBlock.meshID][meshIndex].triangles.Length; j++) {
			AddTri(Main.meshTable[thisBlock.meshID][meshIndex].triangles[j], Main.meshTable[thisBlock.meshID][meshIndex].vertices.Length, e, thisBlock);
		}
		//vertCount += Main.meshTable[thisBlock.meshID][meshIndex].vertices.Length;
	}

	//Ermittelt genau, ob Voxelfaces gerendert werden müssen
	public static bool DrawVoxelFace(Bounds thisBounds, Bounds nextBounds, int dir) {
		return dir switch {
			0 => (thisBounds.min.y != 0f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			1 => (thisBounds.max.y != 1f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			2 => (thisBounds.min.x != 0f) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			3 => (thisBounds.max.x != 1f) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			4 => (thisBounds.min.z != 0f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			5 => (thisBounds.max.z != 1f) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			_ => true
			//Geht noch nich ganz
			/*0 => (thisBounds.min.y + 1 > nextBounds.max.y) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			1 => (thisBounds.max.y - 1 < nextBounds.min.y) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.z < nextBounds.min.z || thisBounds.max.x > nextBounds.max.x || thisBounds.max.z > nextBounds.max.z),
			2 => (thisBounds.min.x + 1 > nextBounds.max.x) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			3 => (thisBounds.max.x - 1 < nextBounds.min.x) || (thisBounds.min.y < nextBounds.min.y || thisBounds.min.z < nextBounds.min.z || thisBounds.max.y > nextBounds.max.y || thisBounds.max.z > nextBounds.max.z),
			4 => (thisBounds.min.z + 1 > nextBounds.max.z) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			5 => (thisBounds.max.z - 1 < nextBounds.min.z) || (thisBounds.min.x < nextBounds.min.x || thisBounds.min.y < nextBounds.min.y || thisBounds.max.x > nextBounds.max.x || thisBounds.max.y > nextBounds.max.y),
			_ => true*/
		};
	}

	void GetMeshVoxel(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		Bounds b = chunk.GetBounds(pos);
		if (!thisBlock.mode) {
			for (int i = 0; i < 6; i++) {
				//prüft ob Fläche sichtbar ist				
				if (chunk.CheckBlock(pos + VD.dirs[i]).type != BType.Voxel || DrawVoxelFace(b, chunk.CheckBounds(pos + VD.dirs[i]), i)) { 
					//definert Ecken der Fläche
					for (int j = 0; j < 4; j++) {
						AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.voxelTris[i, j]], b.size), VD.voxelUvs[j], e, thisBlock);
					}
					//Definert 2 Dreiecke für quadratische Fläche
					for (int k = 0; k < 6; k++) {
						AddTri(VD.vertexTris[k], 4, e, thisBlock);
					}
					//vertCount += 4;
				}
			}
		} else {
			//Slopes
			VoxelData thisAtr = chunk.GetVoxelData(pos);
			int dir;
			if (thisBlock.slabType != 0) dir = thisAtr.data[1] / 2;
			else dir = thisAtr.mainAtr / 2;

			//Fügt gespeicherte Ecken und Dreiecke hinzu
			for (int i = 0; i < VD.slopeTris.Length; i++) {
				AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.vertRots[dir, VD.slopeTris[i]]], b.size), VD.voxelUvs[VD.slopeUVs[i]], e, thisBlock);
				AddTri(0, 1, e, thisBlock);
			}
		}
	}

	void GetMeshCustomSlab(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		int thisAtr = chunk.GetVoxelAtr(pos);
		//Connect?
		//F�gt gespeicherte Ecken und Dreiecke hinzu
		int meshId = thisBlock.meshID;

		for (int i = 0; i < Main.meshTable[meshId][0].vertices.Length; i++) {
			AddVert(RotateVert(Main.meshTable[meshId][0].vertices[i] + new Vector3(0f, 0f, thisAtr % 3 / 3f), VD.dirToRot2[thisAtr / 3]) + pos, Main.meshTable[meshId][0].uv[i], e, thisBlock);
		}
		for (int j = 0; j < Main.meshTable[meshId][0].triangles.Length; j++) {
			AddTri(Main.meshTable[meshId][0].triangles[j], Main.meshTable[meshId][0].vertices.Length, e, thisBlock);
		}
		//vertCount += Main.meshTable[meshId][0].vertices.Length;
	}

	void GetMeshTerrain(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		Block check;
		byte marchIndex = 0;

		for (int i = 0; i < 26; i++) {
			//Pr�ft welcher benachbarte Block leer ist um ID in der Tabelle zu ermitteln
			check = chunk.CheckBlock(pos + VD.dirs[i]);
			if (check.type != BType.Terrain) { // || (check.type == 3 && check.variant == 0))) { // || (VD.tCheck[i].y == -1 && CheckBlock(pos + VD.tCheck[i]).type != 2)) {
				marchIndex |= VD.tBytes[i];
			}
			//Ein schwebender Block wird gel�scht
			if (marchIndex == 255) {
				if (!chunk.Active && pos.y < 40) chunk.SetVoxel(pos, 25, 12);
				else chunk.SetVoxel(pos, 0);
				return;
			}
		}
		if (marchIndex == 0) return;
		//Terrain Flach?
		for (int i = 0; i < 6; i++) {
			if (marchIndex == VD.tBytes[i]) { chunk.SetVoxel(pos, thisBlock.id, 1); break; }
		}

		int edgeIndex = 0;
		int num;

		//Berechnet Position der Ecken aus tabelle
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 3; j++) {
				num = VD.tTriangles[marchIndex, edgeIndex];
				if (num == -1) return;

				AddVert((pos + VD.voxelVerts[VD.tEdges[num, 0]] + (pos + VD.voxelVerts[VD.tEdges[num, 1]]) + Vector3.up) / 2f, Vector2.zero, e, thisBlock);
				AddTri(0, 1, e, thisBlock);
				edgeIndex++;
			}
		}
	}

	void GetMeshRounded(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		Block check;
		byte marchIndex;

		//Optimieren!!!
		Vector3Int newpos;
		for (int k = 0; k < 26; k++) {
			if (chunk.CheckBlock(pos + VD.dirs[k]).type < BType.Rounded) {
				newpos = pos + VD.dirs[k];
				marchIndex = 0;
				for (int i = 0; i < 26; i++) {
					//Pr�ft welcher benachbarte Block leer ist um ID in der Tabelle zu ermitteln
					check = chunk.CheckBlock(newpos + VD.dirs[i]);
					if (check.type == BType.Rounded) {
						marchIndex |= VD.tBytes[i];
					}
					if (marchIndex == 255) {
					//	chunk.SetVoxel(newpos, thisBlock.id);
					}
				}
				if (marchIndex == 0) return;

				//int edgeIndex = 0;
				int num;
				//Berechnet Position der Ecken aus tabelle
				for (int edgeIndex = 15; edgeIndex >= 0; edgeIndex--) {

					num = VD.tTriangles[marchIndex, edgeIndex];
					if (num != -1) {
						AddVert(newpos + (Vector3)(VD.voxelVerts[VD.tEdges[num, 0]] + VD.voxelVerts[VD.tEdges[num, 1]]) / 2f, Vector2.zero, e, thisBlock);
						AddTri(0, 1, e, thisBlock);
					}
				}
			}
		}
	}

	void GetMeshLiquid(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		int height;

		for (int i = 0; i < 6; i++) {
			//definert Ecken der Fl�che							//waterlogging imp0lementieren
			if (chunk.CheckBlock(pos + VD.dirs[i]).type == BType.Air) { // || i == 1 && CheckBlock(pos + VD.faceChecks[i]).type != 7) {
				for (int j = 0; j < 4; j++) {
					Vector3 vert = VD.voxelVerts[VD.voxelTris[i, j]];

					//H�he
					if (vert.y != 0) {
						height = chunk.GetVoxelAtr(pos);
						if (thisBlock.mode) vert -= new Vector3(0, 1 - (height / 16f), 0f);
						else {

							for (int k = 0; k < 8; k++) {
								Vector3Int dir = pos + new Vector3Int(VD.chunkChecks[k].x, 0, VD.chunkChecks[k].y);

								if (chunk.CheckBlock(dir).type == BType.Liquid && (chunk.CheckBlockAtr(dir) * VD.waterHeight[k, VD.voxelTris[i, j]] > height)) {
									height = chunk.CheckBlockAtr(dir);
								}
							}
							vert += (thisBlock.meshID - height) / (float)thisBlock.meshID * Vector3.down;
						}
					}

					//Mesh f�llt benachbarte Bl�cke aus
					for (int k = 2; k < 6; k++) {
						if (chunk.CheckBlock(pos + VD.dirs[k] * -1).type > BType.Liquid) {
							vert += VD.voxelFaces[k, VD.voxelTris[i, j]] * (Vector3)VD.dirs[k];
						}
					}
					AddVert(pos + vert, Vector2.zero, e, thisBlock);
				}

				//Definert 2 Dreiecke f�r quadratische Fl�che
				for (int k = 0; k < 6; k++) {
					AddTri(VD.vertexTris[k], 4, e, thisBlock);
				}
				//vertCount += 4;
			}
		}
	}

	void GetMeshBounds(Vector3Int pos, UpdateEvent e) {
		Block thisBlock = chunk.GetVoxel(pos);
		Bounds b = chunk.GetBounds(pos);
		Block nextBlock;

		for (int i = 0; i < 6; i++) {
			//pr�ft ob Fl�che sichtbar ist			
			nextBlock = chunk.CheckBlock(pos + VD.dirs[i]);
			if (nextBlock.type < BType.Terrain && thisBlock.type != BType.Liquid || nextBlock.type == BType.Air) {
				//definert Ecken der Fl�che
				for (int j = 0; j < 4; j++) {
					AddVert(pos + b.min + Vector3.Scale(VD.voxelVerts[VD.voxelTris[i, j]], b.size), VD.voxelUvs[j], e, thisBlock);
				}
				//Definert 2 Dreiecke f�r quadratische Fl�che
				for (int k = 0; k < 6; k++) {
					AddTri(VD.vertexTris[k], 4, e, thisBlock);
				}
			}
		}
	}

	//F�r implementierung von rotierung
	public static Vector3 RotateVert(Vector3 vert, int dir) {
		Vector3 center = new(0.5f, 0.5f, 0.5f);
		return Quaternion.Euler(VD.dirToRot2[dir]) * (vert - center) + center;
	}
	public static Vector3 RotateVert(Vector3 vert, Vector3Int angle) {
		Vector3 center = new(0.5f, 0.5f, 0.5f);
		return Quaternion.Euler(angle) * (vert - center) + center;
	}
	public static Vector3 RotateVert(Vector3 vert, Quaternion rotation) {
		Vector3 center = new(0.5f, 0.5f, 0.5f);
		return rotation * (vert + center) - center;
	}
}