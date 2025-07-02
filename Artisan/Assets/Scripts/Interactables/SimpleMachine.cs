using UnityEngine;
using System.Collections.Generic;

public class SimpleMachine : Machine
{
    [SerializeField] List<InventoryItem> acceptedItems;
    [SerializeField] List<InventoryItem> productedItems;
    public override List<InventoryItem> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<InventoryItem> ProducedItems
    {
        get { return productedItems; }
        set { ProducedItems = value; }
    }

    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        for (int i = 0; i < acceptedItems.Count; i++)
        {
            if (heldItem.name == acceptedItems[i].name)
            {
                farm.playerInventory.RemoveInventoryItem(heldItem);
                farm.playerInventory.AddInventoryItem(productedItems[i]);
            }
        }

    }
}
