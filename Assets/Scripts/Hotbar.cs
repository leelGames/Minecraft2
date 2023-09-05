using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    public Player player;

    public Sprite[] sprites;
    
    public List<Image> slotObjects;
    public List<Image> itemObjects;
    Item[] slots;
    float scroll;
    int size = 0;

    int selectIndex = 0;

    void Start() {
        slots = new Item[8];
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) addItem(ID.items[(size + 1) % 4]);
        if (Input.GetKeyDown(KeyCode.Y)) removeItem(selectIndex);

        if (size > 0) {
            scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                slotObjects[selectIndex].sprite = sprites[0];
                if (scroll > 0) selectIndex++;
                else selectIndex--;

                if (selectIndex >= size) selectIndex = 0;
                else if (selectIndex < 0) selectIndex = size - 1;

                slotObjects[selectIndex].sprite = sprites[2];
            }
        }
    }

    void addItem(Item item) {
        Debug.Log(item.name);
        if (size < 8) {
           
            slots[size] = item;
            //slotObjects[size].sprite = sprites[0];
            slotObjects[size].enabled = true;
            itemObjects[size].sprite = Main.itemTextures[item.textureID];
            itemObjects[size].enabled = true;
            size++;
        }
    }
    void removeItem(int slot) {
        if (size > 0) {
            
            slots[slot] = null;
            itemObjects[slot].sprite = null;
            itemObjects[slot].enabled = false;
            if (slot == size - 1) {
                while (itemObjects[slot].sprite == null) {
                    size--;
                    slotObjects[slot].enabled = false;
                    slot--;
                }
                selectIndex = slot;
                slotObjects[selectIndex].sprite = sprites[2];
            }
        }
    }

}
