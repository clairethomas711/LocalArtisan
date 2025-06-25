using UnityEngine;

public class SeedBuyerTEMP : Interactable
{
    [SerializeField] InventorySlot seedToBuy;
    public override void Interact(InventorySlot heldItem, FarmManager farm)
    {
        if (farm.money >= seedToBuy.value)
        {
            farm.SubtractMoney(seedToBuy.value);
            farm.AddInventoryItem(seedToBuy);
        }
    }
}
