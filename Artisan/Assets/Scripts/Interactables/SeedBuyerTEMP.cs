using UnityEngine;

public class SeedBuyerTEMP : Interactable
{
    [SerializeField] ItemData seedToBuy;
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        if (farm.money >= seedToBuy.value)
        {
            farm.SubtractMoney(seedToBuy.value);
            farm.playerInventory.AddInventoryItem(seedToBuy.id);
        }
    }
}
