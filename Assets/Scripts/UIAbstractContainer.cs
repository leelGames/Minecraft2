using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UICType {Hotbar, Creative, Survival, Container}
public class UIAbstractContainer : MonoBehaviour {
    public Player player;
    public GameObject slotPrefab;
    public GameObject itemPrefab;

    public UICType type;
    
    protected List<UISlot> slotObjects;

    protected void CreateSlot() {
        UISlot slotObject = Instantiate(slotPrefab, transform).GetComponent<UISlot>();
        slotObject.container = this;
            
        UIItem itemObject = Instantiate(itemPrefab, slotObject.transform).GetComponent<UIItem>();
        slotObject.itemObject = itemObject;

        slotObjects.Add(slotObject);
    }


    /*public void AddSlot() {}

    public void RemoveSlot() {}

    public void SetItem(UISlot slot, UIItem item) {}

    public void RemoveItem(UISlot slot) {}

    public void MoveItem(UISlot from, UISlot to) {}

    public void CopyItem(UISlot from, UISlot to) {}*/

    //Implementieren, dass referenzen immer stimmen, item kennt slot kennt container
    //Hotbar plus wird eigenes Objekt
}
