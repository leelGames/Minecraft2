using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
     public Sprite[] sprites;
    
    public List<Image> slotObjects;
    public List<UIItem> itemObjects;
    public AbstractInventory content;

    public void Load() {
        
    }

}

public abstract class AbstractInventory {
    List<ItemStack> slots;

    public AbstractInventory() {
        slots = new();
    }

    public void add(Item item) {
        slots.Add(new ItemStack(item.id, 1));
    }
}

public class CreativeInventory : AbstractInventory {

}

/*public class SurvivalInventory : AbstractInventory {

}
public class ContainerInventory : AbstractInventory {
}*/
