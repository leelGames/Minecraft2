using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
     public Sprite[] sprites;
    
    public List<UISlot> slotObjects;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public CreativeInventory content;

    void Start() {
        slotObjects = new();
    }

    public void Load() {
        if (!content.loaded) {

            for (int i = 0; i < content.slots.Count; i++) {
                if (slotObjects.Count <= i) {
                    UISlot slotObject = Instantiate(slotPrefab).GetComponent<UISlot>();
                    slotObject.transform.SetParent(transform);
            
                    UIItem itemObject = Instantiate(itemPrefab).GetComponent<UIItem>();
                    slotObject.item = itemObject;
                    itemObject.transform.SetParent(slotObject.transform);

                    itemObject.image.sprite = Main.itemTextures[ID.items[content.slots[i].itemID].textureID];
                    slotObjects.Add(slotObject);
                }
                else if (slotObjects[i].item.image.sprite != Main.itemTextures[content.slots[i].itemID]) {
                    slotObjects[i].item.image.sprite = Main.itemTextures[content.slots[i].itemID];
                }
            }

        }
        content.loaded = true;
    }

}

public class AbstractInventory {
    public List<ItemStack> slots;

    public AbstractInventory() {
        slots = new();
    }

    public void add(Item item) {
        slots.Add(new ItemStack(item.id, 1));
    }
}

public class CreativeInventory : AbstractInventory {
    public bool loaded = false;
    public CreativeInventory() : base() {}
}

/*public class SurvivalInventory : AbstractInventory {

}
public class ContainerInventory : AbstractInventory {
}*/
