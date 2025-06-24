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

    public void UseSeed(GameObject target, Seed currentItem)
    {
        TileBehavior tile = target.GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Plant(currentItem.stages[0], currentItem.stages[1]);
        }
    }
}
