using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ID {
    public static Item[] items = Main.Concat<Item>(new Item[]{
        new ToolItem("Empty", 0)

    }, getBlockItems());
    
    static Item[] getBlockItems() {
        Item[] blockItems = new Item[BD.blocks.Length - 1];
        for (int i = 1; i < blockItems.Length; i++) {
            blockItems[i - 1] = new BlockItem(BD.blocks[i].blockName, BD.blocks[i].textureID, BD.blocks[i].id);
        }
        return blockItems;
    }
}

public class Item {
    static int count = 0;
   
    //ItemEntity entity;
    public int id;
    public string name;
    //Sprite icon; 
    public int textureID;
    int stacklimit;

    public Item(string name, int textureID) {
        this.id = count;
        this.name = name;
        this.textureID = textureID;
        count++;
    }
}

public class BlockItem : Item {
     Block block;

    public BlockItem(string name, int textureID, int blockID) : base(name, textureID) {
        block = BD.blocks[blockID];
    }
}
public class ToolItem : Item {
    public ToolItem(string name, int textureID) : base(name, textureID) {

    }
}


