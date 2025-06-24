using UnityEngine;

//Instantiated by Inventory, stores all functions for using of Items by type.

public class ItemUtility : MonoBehaviour
{
    public void UseHoe(GameObject target)
    {
        TileBehavior tile = target.GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Till();
        }
    }

    public void UseWateringCan(GameObject target)
    {
        TileBehavior tile = target.GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Water();
        }
    }

    public void UseSeed(GameObject target, InventorySlot currentItem)
    {
        TileBehavior tile = target.GetComponent<TileBehavior>();
        if (tile)
        {
            //This isn't working, commenting it out for now :/
            
            //if (currentItem.GetType() == "Seed")
            //    tile.Plant(currentItem.stages[0], currentItem.stages[1]);
        }
    }
}
