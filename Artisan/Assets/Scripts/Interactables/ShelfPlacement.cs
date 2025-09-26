using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : Interactable
{
    [SerializeField] List<Transform> placementLocations;
    [SerializeField] GameObject TEMP_MODEL;
    private string currentAcceptedItem;
    [HideInInspector] public List<InventoryItem> shelfInventory = new List<InventoryItem>();
    StoreManager store;

    void Start()
    {
        store = transform.parent.parent.gameObject.GetComponent<StoreManager>(); //THIS IS NOT GREAT - NEED TO KEEP THIS FORMAT OR CHANGE
    }
    public override string Interact(InventoryItem heldItem)
    {
        if (heldItem.id == "") //If we have an empty hand, remove the item
        {
            if (shelfInventory.Count > 0)
            {
                DataManager.instance.playerInventory.AddInventoryItem(shelfInventory[shelfInventory.Count - 1].id);
                shelfInventory.Remove(shelfInventory[shelfInventory.Count - 1]);
                if (shelfInventory.Count == 0)
                {
                    currentAcceptedItem = null;
                }
            }
        }
        else if (DataManager.instance.manifest[heldItem.id].itemType != itemType.Artisan) //If we are not holding an artisan item, do nothing
        {
            print("Only Artisan items can be sold in the store.");
            return "";
        }
        else if (currentAcceptedItem == null || (currentAcceptedItem != null && shelfInventory.Count <= 0)) //If this is an empty & unassigned shelf, add this as the new item
        { //OR If this is different from our assigned item, but our shelf is empty, reassign
            currentAcceptedItem = heldItem.id;
            shelfInventory.Add(heldItem);
            DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
        }
        else if (currentAcceptedItem == heldItem.id && shelfInventory.Count < placementLocations.Count) //If this is an assigned shelf and we have the correct item
        {
            shelfInventory.Add(heldItem);
            DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
        }
        UpdateDisplay();
        store.UpdateInventory();
        return "";
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < placementLocations.Count; i++)
        {
            if (placementLocations[i].childCount > 0)
            {
                Destroy(placementLocations[i].GetChild(0).gameObject);
            }
        }
        if (currentAcceptedItem == null) { return; }
        Artisan a = (Artisan)DataManager.instance.manifest[currentAcceptedItem];
        for (int i = 0; i < shelfInventory.Count; i++)
        {
            Instantiate(a.model, placementLocations[i].position, placementLocations[i].rotation, placementLocations[i]);
        }
    }
}
