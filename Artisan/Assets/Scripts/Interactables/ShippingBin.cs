using UnityEngine;

public class ShippingBin : Interactable
{
    public override void Interact(InventorySlot heldItem, FarmManager farm)
    {
        if (heldItem.value > 0)
        {
            farm.RemoveInventoryItem(heldItem);
            farm.AddMoney(heldItem.value);
        } else { print(heldItem.name.ToString() + " is not a sellable item. Value of " + heldItem.value.ToString()); }
    }
}
