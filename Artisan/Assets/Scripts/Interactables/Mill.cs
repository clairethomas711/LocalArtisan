using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Mill : Machine
{
    [SerializeField] List<InventorySlot> acceptedItems;
    [SerializeField] List<InventorySlot> productedItems;
    public override List<InventorySlot> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<InventorySlot> ProducedItems
    {
        get { return productedItems; }
        set { ProducedItems = value; }
    }

    public override void Interact(InventorySlot heldItem, FarmManager farm)
    {
        for (int i = 0; i < acceptedItems.Count; i++)
        {
            if (heldItem.name == acceptedItems[i].name)
            {
                farm.RemoveInventoryItem(heldItem);
                farm.AddInventoryItem(productedItems[i]);
            }
        }

    }
}
