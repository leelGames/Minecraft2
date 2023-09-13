using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IPointerDownHandler {
    public Image image;
    public UIItem itemObject;
    public Player player;
    public Sprite[] sprites;

    public void setSprite(int spriteID) {
        image.sprite = sprites[spriteID];
    }

    public ItemStack item {
        get {return itemObject.item;}
        set {itemObject.SetItem(value);}
    }

    public void OnPointerDown(PointerEventData eventData) {
        player.inventoryM.OnSlotClicked(this);
    }
}
