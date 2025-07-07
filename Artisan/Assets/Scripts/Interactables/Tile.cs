using UnityEngine;

public class Tile : Interactable
{
    TileBehavior tile;
    void Start()
    {
        tile = GetComponent<TileBehavior>();
    }

    public override void Interact(InventoryItem currentItem)
    {
        if (tile.state == TileBehavior.TileState.Grown)
        {
            UseHarvest();
            return;
        }
        if (currentItem.id == "") { return; }
        switch (DataManager.instance.manifest[currentItem.id].itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                UseHoe();
                break;

            case itemType.WateringCan:
                UseWateringCan();
                break;

            case itemType.Seed:
                UseSeed((Seed)DataManager.instance.manifest[currentItem.id]);
                break;
        }
    }

    private void UseHoe()
    {
        if (tile.Till())
            DataManager.instance.SubtractStamina(2);
    }

    private void UseWateringCan()
    {
        if (tile.Water())
            DataManager.instance.SubtractStamina(2);
    }

    private void UseSeed(Seed currentItem)
    {
        if (tile.Plant(currentItem))
            DataManager.instance.SubtractStamina(1);  
    }

    private void UseHarvest()
    {
        if (tile.Harvest())
            DataManager.instance.SubtractStamina(1);
    }
}
