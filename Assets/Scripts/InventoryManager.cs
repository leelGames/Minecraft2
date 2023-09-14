using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager {
    Player player;
    static bool isDragging;
    static UIItem currentItem;
    static UISlot currentSlot;

    public InventoryManager(Player player) {
        this.player = player;
    }

    void BeginDrag(UIItem itemObject, UISlot slotObject) {
        isDragging = true;
        currentItem = itemObject;
        currentSlot = slotObject;
        itemObject.BeginDrag();
        player.hotbar.ShowAddSlot();
    }

    void EndDrag() {
        isDragging = false;
        currentItem.EndDrag();
        player.hotbar.HideAddSlot();
    }

    public void OnItemClicked(UIItem itemObject, UISlot slotObject) {
        Debug.Log("Item clicked");

        if (isDragging) {
            if (slotObject.itemObject == currentItem) EndDrag();
            else if (slotObject.container.type == UICType.Hotbar) {
                if (player.hotbar.TrySetItem(slotObject, ID.items[currentItem.item.itemID])) EndDrag();

                if (currentSlot.container.type == UICType.Hotbar) player.hotbar.TryRemoveItem(currentSlot);
            }
 
        }
        else {
            BeginDrag(itemObject, slotObject);
        }
       
    }

    public void OnSlotClicked(UISlot slotObject) {
        Debug.Log("Slotclicked");
        if (isDragging) {
            if (slotObject.container.type == UICType.Hotbar) {
                if (player.hotbar.TrySetItem(slotObject, ID.items[currentItem.item.itemID])) EndDrag();
            }
            else EndDrag();

            if (currentSlot.container.type == UICType.Hotbar) player.hotbar.TryRemoveItem(currentSlot);
        }
    }

    public void OnCInventoryClicked() {
        Debug.Log("Inventory Clicked");
        if (isDragging) {
            EndDrag();
            if (currentSlot.container.type == UICType.Hotbar) player.hotbar.TryRemoveItem(currentSlot);
        }
    }

    public void OnHotbarClicked() {
        Debug.Log("Hotbar Clicked");
        if (isDragging) {
            player.hotbar.AddItem(ID.items[currentItem.item.itemID]);
            EndDrag();

            if (currentSlot.container.type == UICType.Hotbar) player.hotbar.TryRemoveItem(currentSlot);
        }
    }
}
