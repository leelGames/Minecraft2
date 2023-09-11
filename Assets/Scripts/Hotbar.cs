using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    public Player player;
    public Sprite[] sprites;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    
    List<UISlot> slotObjects;
  
    List<Item> slots;
    float scroll;

    int selectIndex = 0;

    void Start() {
        slotObjects = new();
        slots = new();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) addItem(ID.items[slots.Count + 1]);
        if (Input.GetKeyDown(KeyCode.Y)) removeItem(selectIndex);
       
        if (slots.Count > 0) {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                slotObjects[selectIndex].image.sprite = sprites[0];
                if (scroll < 0) selectIndex++;
                else selectIndex--;

                if (selectIndex >= slots.Count) selectIndex = 0;
                else if (selectIndex < 0) selectIndex = slots.Count - 1;

                slotObjects[selectIndex].image.sprite = sprites[2];

                player.highlight.select(slots[selectIndex]);
            }
        }
    }

    void addItem(Item item) {
        if (slots.Count < 20) {
            slots.Add(item);

            UISlot slotObject = Instantiate(slotPrefab).GetComponent<UISlot>();
            slotObject.transform.SetParent(transform);
            
            UIItem itemObject = Instantiate(itemPrefab).GetComponent<UIItem>();
            slotObject.item = itemObject;
            itemObject.transform.SetParent(slotObject.transform);

            itemObject.image.sprite = Main.itemTextures[item.textureID];
            slotObjects.Add(slotObject);

            //player.highlight.select(slots[selectIndex]);

            if (slots.Count == 1) slotObjects[selectIndex].image.sprite = sprites[2];
        }
    }
    void removeItem(int slot) {
        if (slots.Count > 0) {
            slots[slot] = ID.items[0];
            
            slotObjects[slot].item.image.sprite = null;
            slotObjects[slot].item.image.enabled = false;

            if (slot == slots.Count - 1) {
                while (slot >= 0 && slotObjects[slot].item.image.sprite == null) {
                    slots.RemoveAt(slot);
                    UISlot slotobject = slotObjects[slot];
                    slotObjects.RemoveAt(slot);
                    Destroy(slotobject.gameObject);
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
                slotObjects[selectIndex].image.sprite = sprites[2];
            }
        }
    }
}
