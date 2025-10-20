using UnityEngine;

public class BasicShopMenu : ShopMenu
{
    public override void PurchaseItem(ItemData itemToPurchase)
    {
        if (DataManager.instance.money >= itemToPurchase.value)
        {
            DataManager.instance.SubtractMoney(itemToPurchase.value);
            DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(itemToPurchase.id, 1));
        }
    }
}
