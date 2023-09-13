using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager {
    Player player;
    static UIItem dragging;

    public InventoryManager(Player player) {
        this.player = player;
    }

    public void OnItemClicked(UIItem itemObject) {
        Debug.Log("Item clicked");
        if (dragging == null) {
            dragging = itemObject;
            itemObject.BeginDrag();
            player.hotbar.ShowAddSlot();
        }
    }

    public void OnSlotClicked(UISlot slotObject) {
        Debug.Log("Slotclicked");
        if (dragging != null) {
            dragging.EndDrag();
            if (!player.hotbar.TrySetItem(slotObject, dragging)) {
                player.hotbar.HideAddSlot();
            } 
            dragging = null;
           
        }

    }
}
