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
        if (DataManager.instance.SubtractStamina(5))
            tile.Till();
            //tile.Invoke("Till", 0.5f);
    }

    private void UseWateringCan()
    {
        if (DataManager.instance.SubtractStamina(5))
            tile.Water();
            //tile.Invoke("Water", 0.5f);
    }

    private void UseSeed(Seed currentItem)
    {
        if (DataManager.instance.SubtractStamina(1))
            tile.Plant(currentItem);
    }

    private void UseHarvest()
    {
        if (DataManager.instance.SubtractStamina(1))
            tile.Harvest();
    }
}
