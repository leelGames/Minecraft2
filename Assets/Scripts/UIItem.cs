using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIItem : MonoBehaviour {
    
    public Image image;
    public ItemStack item;
    Transform parent;
    bool dragging = false;

    public void SetItem(ItemStack item) {
        this.item = item;
        if (item.itemID == 0) {
            image.sprite = null;
            image.enabled = false;
        }
        else {
            image.sprite = Main.itemTextures[ID.items[item.itemID].textureID];
            image.enabled = true;
        }
    }
    public void Update() {
       if (dragging) transform.position = Input.mousePosition;
    }

    public void BeginDrag() {
       dragging = true;
       image.raycastTarget = false;
       parent = transform.parent;
       transform.SetParent(transform.root);
    }

    public void EndDrag() {
        dragging = false;
        image.raycastTarget = true;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    /*public void OnPointerDown(PointerEventData eventData) {
        player.inventoryM.OnItemClicked(this);
    }*/
}


