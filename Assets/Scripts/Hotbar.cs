using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    public Player player;
    public Sprite[] sprites;
    
    public Image[] slotObjects;
    public Image[] itemObjects;
    Item[] slots;
    float scroll;
    int size = 0;

    int selectIndex = 0;

    void Start() {
        slots = new Item[8];
        for (int i = 0; i < slots.Length; i++) slots[i] = ID.items[0];
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) addItem(ID.items[size % 4 + 1]);
        if (Input.GetKeyDown(KeyCode.Y)) removeItem(selectIndex);

        if (size > 0) {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                slotObjects[selectIndex].sprite = sprites[0];
                if (scroll < 0) selectIndex++;
                else selectIndex--;

                if (selectIndex >= size) selectIndex = 0;
                else if (selectIndex < 0) selectIndex = size - 1;

                slotObjects[selectIndex].sprite = sprites[2];

                player.highlight.select(slots[selectIndex]);
            }
        }
    }

    void addItem(Item item) {
        if (size < 8) {
           
            slots[size] = item;
            slotObjects[size].enabled = true;
            itemObjects[size].sprite = Main.itemTextures[item.textureID];
            itemObjects[size].enabled = true;
            size++;

            player.highlight.select(slots[selectIndex]);
            transform.position += new Vector3(-36, 0, 0);
        }
    }
    void removeItem(int slot) {
        if (size > 0) {
            slots[slot] = ID.items[0];
            
            itemObjects[slot].sprite = null;
            itemObjects[slot].enabled = false;
            if (slot == size - 1) {
                while (slot >= 0 && itemObjects[slot].sprite == null) {
                    size--;
                    slotObjects[slot].sprite = sprites[0];
                    slotObjects[slot].enabled = false;
                    slot--;
                    transform.position += new Vector3(36, 0, 0);
                }
                if (slot < 0) slot = 0;
                selectIndex = slot;
                slotObjects[selectIndex].sprite = sprites[2];
              
            } 
            player.highlight.select(slots[selectIndex]);
        }
    }
}
