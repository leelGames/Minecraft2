using UnityEngine;

public class Highlight : MonoBehaviour {

    public Player player;
    public World world;
    public GameObject face;
    public BoxCollider coll;

    public Vector3Int breakPos;
    public Vector3Int blockPlacePos;
    public Vector3Int terrainPlacePos;
    public Vector3Int slabPlacePos;
    readonly float stepincrement = 0.05f;

    bool tcorrection;
    public int dir12;
    int dir2;
    int dir3;
    public int bslab;
    public int pslab;

	public void PlaceHighlight() {
        dir2 = player.dir4 / 2;
        dir3 = player.dir6 / 2;
        
        Block selected = BD.blocks[player.selected];
        bool hit = false;;
		float step = stepincrement;
		Vector3 pos = new();
		Vector3 lastPos = pos;
        Vector3Int lastVoxel = Vector3Int.FloorToInt(pos);
        Vector3Int voxel = lastVoxel;
        Bounds bounds = new();
		Ray ray = player.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
        
        while (step < player.reach) {
            lastPos = pos;
			pos = ray.GetPoint(step);
            voxel = Vector3Int.FloorToInt(pos);

			if (Vector3Int.FloorToInt(lastPos) != voxel) lastVoxel = Vector3Int.FloorToInt(lastPos);

			bounds = world.GetBounds(voxel);
			bounds.center += voxel;
			if (world.GetVoxel(voxel).type > BType.Liquid && bounds.Contains(pos)) {
                hit = true;
                break;
			}
			step += stepincrement;
            
		}
        if (hit) {

            dir12 = CalcDir12(pos, lastVoxel - voxel);
            face.transform.rotation = Quaternion.Euler((lastVoxel - voxel + Vector3.up) * 90);
            breakPos = voxel;
            blockPlacePos = lastVoxel;
            slabPlacePos = Vector3Int.FloorToInt(lastPos);
            terrainPlacePos = Vector3Int.FloorToInt(lastPos + new Vector3(0.5f, 0, 0.5f));
            face.SetActive(true);

            //highlight für slabs berechenen (richtung und position berechnen)
            Block block = world.GetVoxel(breakPos);
            int slabtype = 0;
            bslab = 0;
            pslab = 0;
            if (selected.type == BType.Voxel) slabtype = selected.slabType;
            if (block.type == BType.Voxel) slabtype = block.slabType;

            if (slabtype != 0) {
                bslab = (int)((dir3 == 1 ? pos.x : dir3 == 2 ? pos.z : pos.y) * (slabtype + 1)) % (slabtype + 1);
                pslab = (int)((dir3 == 1 ? lastPos.x : dir3 == 2 ? lastPos.z : lastPos.y) * (slabtype + 1)) % (slabtype + 1);
            }
            slabtype = 0;
            
            if (selected.type == BType.CustomSlab) slabtype = selected.slabType;
            if (block.type == BType.CustomSlab) slabtype = block.slabType;
            if (slabtype != 0) {
                bslab = (int)((dir2 == 1 ? pos.x : pos.z) * 3) % 3;
                pslab = (int)((dir2 == 1 ? lastPos.x : lastPos.z) * 3) % 3;
            }
            //Korrektur für Terrain
            if (selected.type == BType.Terrain || block.type == BType.Terrain && tcorrection) {
                transform.position = Vector3Int.FloorToInt(pos - new Vector3(0.5f, 0f, 0.5f)) + new Vector3(1f, 0.5f, 1f);
            } else { transform.position = bounds.center; }
            if (selected.type == BType.Rounded || block.type == BType.Rounded) {
                transform.localScale = bounds.size * 2f;
            } else { transform.localScale = bounds.size; }

           
        } else face.SetActive(false);
	}
   
    public void RemoveBlock() {
    	Block selected = BD.blocks[player.selected];
		VoxelData pos = world.GetVoxelData(breakPos);
        tcorrection = false;
        //Rounded
        if (selected.type == BType.Rounded) {
            for (int i = 0; i < 8; i++) {
                if (world.GetVoxel(terrainPlacePos - VD.voxelVerts[i]).type == BType.Rounded) {
                    world.SetVoxel(terrainPlacePos - VD.voxelVerts[i], 0);
                }
            }
        }
		else if (pos.block.type == BType.Voxel && pos.block.slabType != 0) {
            //Halfslabs
			if (!pos.block.mode && pos.block.slabType == 1) {
			    int slab = VD.halfSlabCombiner[bslab + 2, 1 + VD.slabDataToID[pos.mainAtr]];
			    if (slab == -1) world.SetVoxel(breakPos, 0);
			    else world.SetVoxel(breakPos, pos.block.id, (1 + 3 * dir3 + slab));
		    }
			else if (pos.block.mode && pos.block.slabType == 1) {
				int slab = VD.halfSlabCombiner[bslab + 2, 1 + VD.slabDataToID[pos.data[0]]];
				if (slab == -1) world.SetVoxel(breakPos, 0);
				else world.SetVoxel(breakPos, pos.block.id, new byte[] { (byte)(1 + 3 * dir3 + slab), pos.data[1] });
			}
			//Thirdslabs
			else if (!pos.block.mode && pos.block.slabType == 2) {
			    int slab = VD.thirdSlabCombiner[bslab + 3, 1 + VD.slabDataToID[pos.mainAtr]];
			    if (slab == -1) world.SetVoxel(breakPos, 0);
			    else world.SetVoxel(breakPos, pos.block.id, (10 + 6 * dir3 + slab));
            } 
            else if (pos.block.mode && pos.block.slabType == 2) {
				int slab = VD.thirdSlabCombiner[bslab + 3, 1 + VD.slabDataToID[pos.data[0]]];
				if (slab == -1) world.SetVoxel(breakPos, 0);
				else world.SetVoxel(breakPos, pos.block.id, new byte[] { (byte)(10 + 6 * dir3 + slab), pos.data[1] });
			}
		} else {
			world.SetVoxel(breakPos, 0);
		}
        if (Main.s.voxelentities) {
            for (int i = 0; i < 6; i++) {
                world.CreateFallingStructure(breakPos + VD.dirs[i]);
            }
        }
		world.UpdateBlock(breakPos);
        
	}
  
    public void PlaceBlock() {
		Block selected = BD.blocks[player.selected];
        VoxelData pos = world.GetVoxelData(blockPlacePos);
        //Terrain
        if (selected.type == BType.Terrain) {
            tcorrection = true;
            for (int i = 0; i < 8; i++) {
                if (world.GetVoxel(terrainPlacePos - VD.voxelVerts[i]).type == BType.Air) {
                    world.SetVoxel(terrainPlacePos - VD.voxelVerts[i], player.selected);
                }
            }
        } else if (selected.type == BType.Voxel && selected.slabType != 0 && (pos.block.type == BType.Air || pos.block.type == BType.Voxel)) {

            pos = world.GetVoxelData(slabPlacePos);
            //Halfslabs
            if (!selected.mode && selected.slabType == 1 && ((!pos.block.mode && pos.block.slabType == 1) || pos.block.type == BType.Air)) {
                int slab = VD.halfSlabCombiner[pslab, 1 + VD.slabDataToID[pos.mainAtr]];
                if (slab == -1) return;
                world.SetVoxel(slabPlacePos, player.selected, (1 + 3 * dir3 + slab));
            } else if (selected.mode && selected.slabType == 1 && ((pos.block.mode && pos.block.slabType == 1) || pos.block.type == BType.Air)) {
                int slab = VD.halfSlabCombiner[pslab, 1 + VD.slabDataToID[pos.data.Length == 2 ? pos.data[0] : 0]];
                if (slab == -1) return;
                world.SetVoxel(slabPlacePos, player.selected, new byte[] { (byte)(1 + 3 * dir3 + slab), (byte)(pos.data.Length == 2 ? pos.data[1] : dir12 * 2) });
            }
            //Thirdslabs
            else if (!selected.mode && selected.slabType == 2 && ((!pos.block.mode && pos.block.slabType == 2) || pos.block.type == BType.Air)) {
                int slab = VD.thirdSlabCombiner[pslab, 1 + VD.slabDataToID[pos.mainAtr]];
                if (slab == -1) return;
                world.SetVoxel(slabPlacePos, player.selected, (10 + 6 * dir3 + slab));
            } else if (selected.mode && selected.slabType == 2 && ((pos.block.mode && pos.block.slabType == 2) || pos.block.type == BType.Air)) {
                int slab = VD.thirdSlabCombiner[pslab, 1 + VD.slabDataToID[pos.data.Length == 2 ? pos.data[0] : 0]];
                if (slab == -1) return;
                world.SetVoxel(slabPlacePos, player.selected, new byte[] { (byte)(10 + 6 * dir3 + slab), (byte)(pos.data.Length == 2 ? pos.data[1] : dir12 * 2) });
            } else { Debug.Log("wron slab"); }
        } else if (selected.type == BType.Liquid) world.SetVoxel(blockPlacePos, player.selected, 12);
        else if (selected.type == BType.CustomSlab) world.SetVoxel(slabPlacePos, player.selected, 3 * player.dir4 + (player.dir4 == 0 || player.dir4 == 2 ? pslab : 2 - pslab));
        else if (selected.rotMode == RMode.Slope) world.SetVoxel(blockPlacePos, player.selected, dir12 * 2);
        else if (selected.rotMode == RMode.AllAxis6) world.SetVoxel(blockPlacePos, player.selected, player.dir6 * 4);
        else if (selected.rotMode == RMode.YAxis) world.SetVoxel(blockPlacePos, player.selected, player.dir4);
        else if (selected.rotMode == RMode.AllAxis3) world.SetVoxel(blockPlacePos, player.selected, dir3 * 8);
        else {
            world.SetVoxel(blockPlacePos, player.selected);
        }
        if (Main.s.voxelentities) {
            world.CreateFallingStructure(blockPlacePos);
        }
		world.UpdateBlock(blockPlacePos);
	}

    int CalcDir12(Vector3 pos, Vector3Int norm) {
        //Rotate 12 Achsen

		float[] dist = new float[4];
        int index = 0;

        if (norm == Vector3Int.down || norm == Vector3Int.up) {
            for (int i = 0; i < 4; i++) {
                dist[i] = Vector2.Distance(new Vector2(breakPos.x, breakPos.z) + VD.dir12Table[i], new Vector2(pos.x, pos.z));
            }
            if (norm == Vector3Int.up) index = 4;
            else index = 0;
        } else if (norm == Vector3Int.left || norm == Vector3Int.right) {
            for (int i = 0; i < 4; i++) {
                dist[i] = Vector2.Distance(new Vector2(breakPos.y, breakPos.z) + VD.dir12Table[i], new Vector2(pos.y, pos.z));
            }
            if (norm == Vector3Int.right) index = 12;
            else index = 8;
        } else if (norm == Vector3Int.forward || norm == Vector3Int.back) {
            for (int i = 0; i < 4; i++) {
                dist[i] = Vector2.Distance(new Vector2(breakPos.x, breakPos.y) + VD.dir12Table[i], new Vector2(pos.x, pos.y));
            }
            if (norm == Vector3Int.back) index = 20;
            else index = 16;
        } 
        return VD.slopeDirs[index + MinIndex(dist)];
    }
    
    int MinIndex(float[] array) {
        int min = 0;

        for (int i = 0; i < array.Length; i++) {
            if (array[i] < array[min]) min = i;
        }
        return min;
    }
	
}
