using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ID {

    static int[] blockItemTexIDs = new int[] {0, 0, 1, 2, 3, 0, 0, 0, 11, 12, 21, 22, 2, 4, 5, 6, 7, 8, 9, 18, 16, 17, 20, 13, 14, 15, 10, 20, 19, 19};

    public static Item[] items = getBlockItems();
    
    static Item[] getBlockItems() {
      
        Item[] blockItems = new Item[BD.blocks.Length - 1];
        for (int i = 0; i < blockItems.Length; i++) {
            blockItems[i] = new Item(BD.blocks[i].blockName, blockItemTexIDs[i]);
            blockItems[i].block = BD.blocks[i];
            BD.blocks[i].item = blockItems[i];
        }
        return blockItems;
    }

}

public class Item {
    static int count = 0;
   
    public int id;
    public string name;

    public Block block;
   
    //ItemEntity entity;
    public int textureID;
    int stacklimit;

    public Item(string name, int textureID) {
        this.id = count;
        this.name = name;
        this.textureID = textureID;
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


