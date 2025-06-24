using UnityEngine;

public class Tile : Interactable
{
    public override void Interact(InventorySlot currentItem, FarmManager farm)
    {
        switch (currentItem.itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                UseHoe();
                break;

            case itemType.WateringCan:
                UseWateringCan();
                break;

            case itemType.Seed:
                UseSeed((Seed)currentItem);
                break;
        }
    }
    
    private void UseHoe()
    {
        TileBehavior tile = GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Till();
        }
    }

    private void UseWateringCan()
    {
        TileBehavior tile = GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Water();
        }
    }

    private void UseSeed(Seed currentItem)
    {
        TileBehavior tile = GetComponent<TileBehavior>();
        if (tile)
        {
            tile.Plant(currentItem.stages[0], currentItem.stages[1]);
        }
    }
}
