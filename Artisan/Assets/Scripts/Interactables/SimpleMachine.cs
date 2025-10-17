using UnityEngine;
using System.Collections.Generic;

public class SimpleMachine : Machine
{
    [SerializeField] int processingTimeInMinutes;
    [SerializeField] List<ItemData> acceptedItems;
    [SerializeField] List<ItemData> producedItems;
    public override List<ItemData> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<ItemData> ProducedItems
    {
        get { return producedItems; }
        set { ProducedItems = value; }
    }

    public override string Interact(InventoryItem heldItem)
    {
        if (heldItem.id != "" && AttemptToMove(DataManager.instance.manifest[heldItem.id].itemType))
        {
            Tile t = transform.parent.gameObject.GetComponent<Tile>();
            DataManager.instance.playerInventory.AddInventoryItem(t.tileInventoryId);
            t.ClearTile();
            Destroy(gameObject);
            return "Hit";      
        }
        if (state == MachineState.ready)
        {
            for (int i = 0; i < acceptedItems.Count; i++)
            {
                if (heldItem.id == acceptedItems[i].id)
                {
                    DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
                    StartProducing(producedItems[i]);
                    return "";
                }
            }
        }
        else if (state == MachineState.produced)
        {
            TakeProducedItem();
        }
        return "";
    }

    public override int CalculateProcessingTime()
    {
        return processingTimeInMinutes;
    }

}
