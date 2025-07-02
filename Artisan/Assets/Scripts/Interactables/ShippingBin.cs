using UnityEngine;

public class ShippingBin : Interactable
{
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        if (heldItem.value > 0)
        {
            farm.playerInventory.RemoveInventoryItem(heldItem);
            farm.AddMoney(heldItem.value);
        } else { print(heldItem.name.ToString() + " is not a sellable item. Value of " + heldItem.value.ToString()); }
    }
}
