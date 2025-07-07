using UnityEngine;

public class SeedBuyerTEMP : Interactable
{
    [SerializeField] ItemData seedToBuy;
    public override void Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.money >= seedToBuy.value)
        {
            DataManager.instance.SubtractMoney(seedToBuy.value);
            DataManager.instance.playerInventory.AddInventoryItem(seedToBuy.id);
        }
    }
}
