using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : Interactable
{
    [SerializeField] List<Transform> placementLocations;
    [SerializeField] GameObject TEMP_MODEL;
    private string currentAcceptedItem;
    private List<InventoryItem> shelfInventory = new List<InventoryItem>();
    public override void Interact(InventoryItem heldItem)
    {
        if (heldItem.id == "")
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
        else if (currentAcceptedItem == null)
        {
            currentAcceptedItem = heldItem.id;
            shelfInventory.Add(heldItem);
            DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
        }
        else if (currentAcceptedItem == heldItem.id && shelfInventory.Count < placementLocations.Count)
        {
            shelfInventory.Add(heldItem);
            DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
        }
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < placementLocations.Count; i++)
        {
            if (placementLocations[i].childCount > 0)
            {
                Destroy(placementLocations[i].GetChild(0).gameObject);
            }
        }
        for (int i = 0; i < shelfInventory.Count; i++)
        {
            Instantiate(TEMP_MODEL, placementLocations[i].position, placementLocations[i].rotation, placementLocations[i]);
        }
    }
}
