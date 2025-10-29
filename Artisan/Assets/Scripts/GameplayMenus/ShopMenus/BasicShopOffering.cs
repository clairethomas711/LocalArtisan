using UnityEngine;

public class BasicShopOffering : ShopOffering
{
    public override void PurchaseItem()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        if (DataManager.instance.money >= o.defaultValue)
        {
            DataManager.instance.SubtractMoney(o.defaultValue);
            DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(o.id, 1));
        }
    }
}
