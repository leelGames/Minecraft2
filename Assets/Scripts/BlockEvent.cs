using System.Collections.Generic;
using UnityEngine;

public class DynamicWater : Block {
	public int amount;
	public int layers;
	public int delay;

	System.Random random;

	public DynamicWater(string name, int texture, int flowAmount, int layer, int speed) : base(name, texture, 4, layer, 0, BType.Liquid, CMode.None, RMode.None, SMode.UVAlpha, false, true, new string[] { "Waterheight" }) {
		amount = flowAmount;
		layers = layer;
		delay = speed;
		random = new();
	}

	public override bool OnBlockUpdate(Chunk chunk, Vector3Int pos) {
		int volume = chunk.GetVoxelAtr(pos);
		int prevolume = volume;
		int flowAmount = amount;

		//runterfallen
		int volumedown = -1;
		int temp;
		Block nextBlock = chunk.CheckBlock(pos + Vector3Int.down);
		int nextAtr = chunk.CheckBlockAtr(pos + Vector3Int.down);

		if (nextBlock.type == BType.Air) {
			volumedown = volume;
			volume = 0;
		} else if (nextBlock.type == BType.Liquid) {
			if (nextBlock.id != id && volume == 1) {
				volume = 0;
				volumedown = -1;
			} else if (nextAtr < layers) {
				temp = nextAtr + volume;
				if (temp > layers) {
					volumedown = layers;
					volume = temp - layers;
				} else {
					volumedown = temp;
					volume = 0;
				}
			}
		}
		//fließen
		int minvolume = chunk.world.IsFlat(pos + chunk.Position + Vector3Int.down) ? 1 : 0;

		if (volume > minvolume && (volumedown <= 0 || volumedown == layers)) {
			int[] volumes = new int[4];

			for (int j = 0; j < 4; j++) {
				nextBlock = chunk.CheckBlock(pos + VD.dirs[j + 2]);
				nextAtr = chunk.CheckBlockAtr(pos + VD.dirs[j + 2]);

				if (nextBlock.id == id) {
					volumes[j] = nextAtr;
				} else if (nextBlock.type != BType.Air) volumes[j] = -1;
			}

			List<int> flows = new();
			int r;

			for (int i = 0; i < volume; i++) {
				flows.Clear();
				for (int j = 0; j < 4; j++) {
					if (volumes[j] == i) flows.Add(j);
				}
				if (flows.Count == 0) continue;
				while (flows.Count > 0 && volume > minvolume && flowAmount > 0) {
					r = flows[Mathf.FloorToInt((float)random.NextDouble() * flows.Count)];
					volumes[r]++;
					volume--;
					flowAmount--;
					flows.Remove(r);
				}

				if (volume <= minvolume || flowAmount == 0) break;
			}

			for (int j = 0; j < 4; j++) {						
				if (volumes[j] > 0 && volumes[j] <= layers /*&& chunk.CheckBlock(pos + VD.dirs[j + 2]).slabType == 0*/) chunk.world.SetVoxel(pos + chunk.Position + VD.dirs[j + 2], id, volumes[j]);
			}
		}
		if (volumedown > 0) chunk.world.SetVoxel(pos + chunk.Position + Vector3Int.down, id, volumedown);

		if (volume != prevolume) {
			if (volume == 0) chunk.SetVoxel(pos, 0);
			else chunk.SetVoxel(pos, id, volume);

			chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position, delay));
			for (int i = 0; i < 6; i++) {
				chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position + VD.dirs[i], delay));
			}
			return true;
		}
		return false;
	}
}

public class StaticWater : Block {
	DynamicWater dynamic;

	public StaticWater(string name, int texture, DynamicWater dynamicWater) : base(name, texture, 4, 0, 1, BType.Liquid, CMode.None, RMode.None, SMode.Water, false, true, new string[] { "Waterheight" }) {
		this.dynamic = dynamicWater;
	}
	public override bool OnBlockUpdate(Chunk chunk, Vector3Int pos) {
		Block nextBlock = chunk.CheckBlock(pos + Vector3Int.up);
		bool canSpread = nextBlock.type == BType.Air || nextBlock.id == id;
		int nextAtr;
		bool update = false;
		
		int level = chunk.GetVoxelAtr(pos);

		for (int i = 2; i < 6; i++) {
			nextBlock = chunk.CheckBlock(pos + VD.dirs[i]);
			nextAtr = chunk.CheckBlockAtr(pos + VD.dirs[i]);
			
			if (nextBlock.type == BType.Air || (nextBlock.id == dynamic.id && nextAtr < level)) {
				chunk.world.SetVoxel(pos + chunk.Position + VD.dirs[i], dynamic.id, level);
				chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position + VD.dirs[i], dynamic.delay));
				update = true;
			} else if (canSpread && nextBlock.id == dynamic.id && nextAtr >= level - 1) {
				chunk.world.SetVoxel(pos + chunk.Position + VD.dirs[i], id, level);
				chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position + VD.dirs[i], dynamic.delay));
				update = true;
			}
		}
		nextBlock = chunk.CheckBlock(pos + Vector3Int.down);
		nextAtr = chunk.CheckBlockAtr(pos + Vector3Int.down);

		if (nextBlock.type == BType.Air || (nextBlock.id == dynamic.id && nextAtr < dynamic.layers)) {
			chunk.world.SetVoxel(pos + chunk.Position + Vector3Int.down, dynamic.id, dynamic.layers);
			chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position+ Vector3Int.down, dynamic.delay));
			update = true;
		} else if (canSpread && nextBlock.id == dynamic.id && nextAtr >= dynamic.layers) {
			chunk.world.SetVoxel(pos + chunk.Position + Vector3Int.down, id, level);
			chunk.world.AddBlockEvent(new BlockEvent(pos + chunk.Position + Vector3Int.down, dynamic.delay));
			update = true;
		}
		return update;
	}
}

public class BlockEvent {
	public Vector3Int pos;
	public int time;

	public BlockEvent(Vector3Int pos, int delay) {
		this.pos = pos;
		this.time = World.currend.time + delay;
	}

	/*public BlockEvent Tick() {
		return new BlockEvent(pos, delay - 1);
	}*/
}
