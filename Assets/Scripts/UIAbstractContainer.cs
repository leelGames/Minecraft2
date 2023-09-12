using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAbstractContainer : MonoBehaviour {
    public Player player;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    
    protected List<UISlot> slotObjects;

    protected void CreateSlot() {
        UISlot slotObject = Instantiate(slotPrefab).GetComponent<UISlot>();
        slotObject.transform.SetParent(transform);
            
        UIItem itemObject = Instantiate(itemPrefab).GetComponent<UIItem>();
        slotObject.itemObject = itemObject;
        itemObject.player = player;
        itemObject.transform.SetParent(slotObject.transform);

        slotObjects.Add(slotObject);
    }
}
