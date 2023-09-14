using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hotbar : UIAbstractContainer, IPointerDownHandler {
    List<Item> slots;

    public GameObject plusSlot;
   
    float scroll;
    int selectIndex = 0;

    public void Start() {
        slotObjects = new();
        slots = new();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) AddItem(ID.items[slots.Count + 1]);
        if (Input.GetKeyDown(KeyCode.Y)) RemoveItem(selectIndex);
       
        if (slots.Count > 0) {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                slotObjects[selectIndex].setSprite(0);
                if (scroll < 0) selectIndex++;
                else selectIndex--;

                if (selectIndex >= slots.Count) selectIndex = 0;
                else if (selectIndex < 0) selectIndex = slots.Count - 1;

                slotObjects[selectIndex].setSprite(2);

                player.highlight.select(slots[selectIndex]);
            }
        }
    }

    public bool TrySetItem(UISlot slot, Item item) {
        for (int i = 0; i < slots.Count; i++) {
            if (slotObjects[i] == slot) {
                SetItem(i, item);
                return true;
            }
        }
        return false;
    }

    public bool TryRemoveItem(UISlot slot) {
        for (int i = 0; i < slots.Count; i++) {
            if (slotObjects[i] == slot) {
                RemoveItem(i);
                return true;
            }
        }
        return false;
    }

    public void SetItem(int slot, Item item) {
        if (slot < slots.Count) {
            slots[slot] = item;
            slotObjects[slot].item = new ItemStack(item.id, 1);
            if (slot == selectIndex) player.highlight.select(slots[selectIndex]);
        }
        else {
            AddItem(item);
        }
    }

    public void AddItem(Item item) {
        if (slots.Count < 8) {
            slots.Add(item);
            if (slotObjects.Count < slots.Count) {
                CreateSlot();
                plusSlot.transform.SetAsLastSibling();
            }
            else {slotObjects[slots.Count - 1].gameObject.SetActive(true);}
          
            slotObjects[slots.Count - 1].item = new ItemStack(item.id, 1);
           
            if (slots.Count == 1) {
                player.highlight.select(slots[selectIndex]);
                slotObjects[selectIndex].setSprite(2);
            }
        }
    }

    void RemoveItem(int slot) {
        if (slots.Count > 0) {
            slots[slot] = ID.items[0];
            slotObjects[slot].item = new ItemStack(0, 0);

            if (slot == slots.Count - 1) {
                while (slot >= 0 && slotObjects[slot].item.itemID == 0) {
                    slots.RemoveAt(slot);
                    slotObjects[slot].setSprite(0);
                    slotObjects[slot].gameObject.SetActive(false);
                    
                    slot--;
                }
                selectIndex = slot;
            }
            if (slot < 0) {
                selectIndex = 0;
                player.highlight.select(ID.items[0]);
            }
            else { 
                player.highlight.select(slots[selectIndex]);
                slotObjects[selectIndex].setSprite(2);
            }
        }
    }

    public void ShowAddSlot() {
       if (slots.Count < 8) plusSlot.SetActive(true);
    }
    public void HideAddSlot() {
      plusSlot.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData) {
        player.inventoryM.OnHotbarClicked();
    }
}
