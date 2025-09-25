using UnityEngine;
using System.Collections.Generic;

public class SimpleMachine : Machine
{
    [SerializeField] List<ItemData> acceptedItems;
    [SerializeField] List<ItemData> productedItems;
    public override List<ItemData> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<ItemData> ProducedItems
    {
        get { return productedItems; }
        set { ProducedItems = value; }
    }

    public override void Interact(InventoryItem heldItem)
    {
        if (state == MachineState.ready)
        {
            for (int i = 0; i < acceptedItems.Count; i++)
            {
                if (heldItem.id == acceptedItems[i].id)
                {
                    DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
                    StartProducing(productedItems[i]);
                }
            }
        }
        else if (state == MachineState.produced)
        {
            TakeProducedItem();
        }
    }

    public override int CalculateProcessingTime(int minOfProductionStart)
    {
        return 0;
    }

}
