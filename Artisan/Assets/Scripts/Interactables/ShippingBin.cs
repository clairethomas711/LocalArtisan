using UnityEngine;

public class ShippingBin : Interactable
{
    public override void Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].value > 0)
        {
            DataManager.instance.playerInventory.RemoveInventoryItem(heldItem.id);
            DataManager.instance.AddMoney(DataManager.instance.manifest[heldItem.id].value);
        } else { print(heldItem.id + " is not a sellable item. Value of " + DataManager.instance.manifest[heldItem.id].value.ToString()); }
    }
}
