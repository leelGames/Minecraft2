using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    public Player player;
    public Image image;
    Sprite icon;
    public ItemStack item;

    Vector3 position;
    Transform parent;

    public void SetItem(ItemStack item) {
        this.item = item;
        if (item.itemID == 0) {
            image.sprite = null;
            image.enabled = false;
        }
        else {
            image.sprite = Main.itemTextures[item.itemID - 1];
            image.enabled = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
       image.raycastTarget = false;
       position = transform.position;
       parent = transform.parent;
       transform.SetParent(transform.root);
       player.hotbar.OnStartDrag();
    }

    public void OnDrag(PointerEventData eventData) {
       transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        image.raycastTarget = true;
        transform.SetParent(parent);
        transform.position = position;
        player.hotbar.OnEndDrag();
    }
}


