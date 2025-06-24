using UnityEngine;

public class Tile : Interactable
{
    TileBehavior tile;
    void Start()
    {
        tile = GetComponent<TileBehavior>();
    }

    public override void Interact(InventorySlot currentItem, FarmManager farm)
    {
        if (tile.state == TileBehavior.TileState.Grown)
        {
            UseHarvest();
            return;
        }

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
        tile.Till();
    }

    private void UseWateringCan()
    {
        tile.Water();
    }

    private void UseSeed(Seed currentItem)
    {
        tile.Plant(currentItem.stages[0], currentItem.stages[1], currentItem.product);
    }

    private void UseHarvest()
    {
        tile.Harvest();
    }
}
