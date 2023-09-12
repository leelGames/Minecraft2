using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ID {
    public static Item[] items = Main.Concat<Item>(new Item[]{
        new Item("Empty", 0)

    }, getBlockItems());
    
    static Item[] getBlockItems() {
        Item[] blockItems = new Item[BD.blocks.Length - 1];
        for (int i = 0; i < blockItems.Length; i++) {
            blockItems[i] = new Item(BD.blocks[i + 1].blockName, BD.blocks[i + 1].id);
        }
        return blockItems;
    }
}

public class Item {
    static int count = 0;
   
    public int id;
    public string name;
    public int blockID;
     //ItemEntity entity;
    //public int textureID;
    int stacklimit;

    public Item(string name, int blockID) {
        this.id = count;
        this.name = name;
        this.blockID = blockID;
        count++;
    }
}

public struct ItemStack {
    public int itemID;
    public byte count;

    public ItemStack(int id, byte count) {
        this.itemID = id;
        this.count = count;
    }
}

/*public class BlockItem : Item {
     Block block;

    public BlockItem(string name, int textureID, int blockID) : base(name, textureID) {
        block = BD.blocks[blockID];
    }
}
public class ToolItem : Item {
    public ToolItem(string name, int textureID) : base(name, textureID) {

    }
}
*/

