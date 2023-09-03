using System.Collections.Generic;
using UnityEngine;

public class DynamicWater : Block {
	public int amount;
	public int layers;
	public int delay;

	public DynamicWater(int flowAmount, int layer, int speed) : base("Water", 12, 4, layer, 0, BType.Liquid, CMode.None, RMode.None, SMode.Water, false, true, new string[] { "Waterheight" }) {
		amount = flowAmount;
		layers = layer;
		delay = speed;
	}

	public override bool OnBlockUpdate(World world, Vector3Int pos) {
		VoxelData data = world.GetVoxelData(pos);
		VoxelData next;
		int volume = data.mainAtr;
		int prevolume = volume;

		int flowAmount = amount;

		//runterfallen
		int volumedown = -1;
		int temp;
		next = world.GetVoxelData(pos + Vector3Int.down);
		if (next.block.type == BType.Air) {
			volumedown = volume;
			volume = 0;
		} else if (next.block.type == BType.Liquid) {
			if (next.block.slabType == 1 && volume == 1) {
				volume = 0;
				volumedown = -1;
			} else if (next.mainAtr < layers) {
				temp = next.mainAtr + volume;
				if (temp > layers) {
					volumedown = layers;
					volume = temp - layers;
				} else {
					volumedown = temp;
					volume = 0;
				}
			}
		}
		//flie�en
		int minvolume = world.IsFlat(pos + Vector3Int.down) ? 1 : 0;

		if (volume > minvolume && (volumedown <= 0 || volumedown == layers)) {
			int[] volumes = new int[4];

			for (int j = 0; j < 4; j++) {
				next = world.GetVoxelData(pos + VD.dirs[j + 2]);
				if (next.block.type == BType.Liquid) {
					//if (next.block.mode) volumes[j] = -1;
					//else 
						volumes[j] = next.mainAtr;
				} else if (next.block.type != BType.Air) volumes[j] = -1;
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
					r = flows[Random.Range(0, flows.Count)];
					volumes[r]++;
					volume--;
					flowAmount--;
					flows.Remove(r);
				}

				if (volume <= minvolume || flowAmount == 0) break;
			}

			for (int j = 0; j < 4; j++) {						//!
				if (volumes[j] > 0 && volumes[j] <= layers && world.GetVoxel(pos + VD.dirs[j + 2]).slabType == 0) world.SetVoxel(pos + VD.dirs[j + 2], id, volumes[j]);
			}
		}
		if (volumedown > 0) world.SetVoxel(pos + Vector3Int.down, id, volumedown);

		if (volume != prevolume) {
			if (volume == 0) world.SetVoxel(pos, 0, 0);
			else world.SetVoxel(pos, id, volume);

			world.AddBlockEvent(new BlockEvent(pos, delay));
			for (int i = 0; i < 6; i++) {
				world.AddBlockEvent(new BlockEvent(pos + VD.dirs[i], delay));
			}
			return true;
		}
		return false;
	}
}

public class StaticWater : Block {
	public byte dynamicLevel;

	public StaticWater(byte dynamicWater) : base("Water Source", 12, 4, 0, 1, BType.Liquid, CMode.None, RMode.None, SMode.Water, false, true, new string[] { "Waterheight" }) {
		this.dynamicLevel = dynamicWater;
	}
	public override bool OnBlockUpdate(World world, Vector3Int pos) {
		VoxelData next;
		DynamicWater dynamic = (DynamicWater)BD.blocks[dynamicLevel];
		bool update = false;

		int level = world.GetVoxelData(pos).mainAtr;

		//if (world.GetBlock(pos + Vector3Int.down).type == 7 && world.GetBlockAtr(pos + Vector3Int.down) == 16) world.SetBlock(pos + Vector3Int.down, id);

		//if (world.GetBlock(pos + Vector3Int.up).type == 0) {
		for (int i = 2; i < 6; i++) {
			next = world.GetVoxelData(pos + VD.dirs[i]);
			if (next.block.type == BType.Air || (next.block.id == dynamic.id && next.mainAtr < level)) {
				world.SetVoxel(pos + VD.dirs[i], dynamic.id, level);
				world.AddBlockEvent(new BlockEvent(pos + VD.dirs[i], dynamic.delay));
				update = true;
			} else if (next.block.id == dynamic.id && next.mainAtr >= level - 1) {
				world.SetVoxel(pos + VD.dirs[i], id, level);
				world.AddBlockEvent(new BlockEvent(pos + VD.dirs[i], dynamic.delay));
				update = true;
			}
		}
		next = world.GetVoxelData(pos + Vector3Int.down);
		if (next.block.type == BType.Air || (next.block.id == dynamic.id && next.mainAtr < dynamic.layers)) {
			world.SetVoxel(pos + Vector3Int.down, dynamic.id, dynamic.layers);
			world.AddBlockEvent(new BlockEvent(pos + Vector3Int.down, dynamic.delay));
			update = true;
		} else if (next.block.id == dynamic.id && next.mainAtr >= dynamic.layers) {
			world.SetVoxel(pos + Vector3Int.down, id, level);
			world.AddBlockEvent(new BlockEvent(pos + Vector3Int.down, dynamic.delay));
			update = true;
		}
		//}
		return update;

	}
}

public struct BlockEvent {
	public Vector3Int pos;
	public int delay;

	public BlockEvent(Vector3Int pos, int delay) {
		this.pos = pos;
		this.delay = delay;
	}

	public BlockEvent Tick() {
		return new BlockEvent(pos, delay - 1);
	}
}
