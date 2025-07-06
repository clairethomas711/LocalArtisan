using UnityEngine;

public class ShippingBin : Interactable
{
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        if (farm.manifest[heldItem.id].value > 0)
        {
            farm.playerInventory.RemoveInventoryItem(heldItem.id);
            farm.AddMoney(farm.manifest[heldItem.id].value);
        } else { print(heldItem.id + " is not a sellable item. Value of " + farm.manifest[heldItem.id].value.ToString()); }
    }
}
