using UnityEngine;

public class BasicShopOffering : ShopOffering
{
    public override void PurchaseItem()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        if (DataManager.instance.money >= o.value)
        {
            DataManager.instance.SubtractMoney(o.value);
            DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(o.id, 1));
        }
    }
}
