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
            UseHarvest(farm);
            return;
        }

        switch (currentItem.itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                UseHoe(farm);
                break;

            case itemType.WateringCan:
                UseWateringCan(farm);
                break;

            case itemType.Seed:
                UseSeed((Seed)currentItem, farm);
                break;
        }
    }

    private void UseHoe(FarmManager farm)
    {
        if(farm.SubtractStamina(5))
            tile.Invoke("Till", 0.5f);
    }

    private void UseWateringCan(FarmManager farm)
    {
        if (farm.SubtractStamina(5))
            tile.Invoke("Water", 0.5f);
    }

    private void UseSeed(Seed currentItem, FarmManager farm)
    {
        if (farm.SubtractStamina(1))
            tile.Plant(currentItem);
    }

    private void UseHarvest(FarmManager farm)
    {
        if (farm.SubtractStamina(1))
            tile.Harvest();
    }
}
