using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : UIAbstractContainer, IPointerDownHandler {
    
    public CreativeInventory content;

    public void Init() {
        slotObjects = new();
        content = player.GetCreativeInventory();
    }

    public void Load() {
        if (!content.loaded) {

            for (int i = 0; i < content.slots.Count; i++) {
                if (slotObjects.Count <= i) {
                   CreateSlot();
                }
                slotObjects[i].item = content.slots[i];
            }
        }
        content.loaded = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        player.inventoryM.OnCInventoryClicked();
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
