using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : UIAbstractContainer {
    List<Item> slots;
   
    float scroll;
    int selectIndex = 0;
    int size;

    public void Start() {
        slotObjects = new();
        slots = new();
        size = 0;
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
    void AddUISlot() {
        if (slotObjects.Count <= size) {
            CreateSlot();
        }
        else //if (slotObjects[size].item.itemID == 0) {
            slotObjects[size].gameObject.SetActive(true);
       // } 
        size++;
    }
    void SetItem(int slot, Item item) {
        if (slots.Count <= slot) {
            slots[slot] = item;
            slotObjects[slot].item = new ItemStack(item.id, 1);
        }
        else AddItem(item);
    }
    void AddItem(Item item) {
        if (size < 20) {
            slots.Add(item);
            AddUISlot();
           
            slotObjects[size - 1].item = new ItemStack(item.id, 1);
           

            if (slots.Count == 1) {
                player.highlight.select(slots[selectIndex]);
                slotObjects[selectIndex].setSprite(2);
            }
        }
    }
    void RemoveUISlot(int slot) {
        slotObjects[slot].setSprite(0);
        slotObjects[slot].gameObject.SetActive(false);
        size--;
        //slotObjects[slot].icon = null;
    }

    void RemoveItem(int slot) {
        if (size > 0) {
            slots[slot] = ID.items[0];
            slotObjects[slot].item = new ItemStack(0, 0);

            if (slot == size - 1) {
                while (slot >= 0 && slotObjects[slot].item.itemID == 0) {
                    slots.RemoveAt(slot);
                    RemoveUISlot(slot);
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

    public void OnStartDrag() {
        AddUISlot();
        slotObjects[size - 1].setSprite(1);
    }
    public void OnEndDrag() {
        RemoveUISlot(size - 1);
    }

    public void TryAddItem(UISlot slot, UIItem itemObject) {
        if (slot == slotObjects[size - 1]) SetItem(slots.Count - 1, ID.items[itemObject.item.itemID]);
    }
}
