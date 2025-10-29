using UnityEngine;

public class UpgradeShopOffering : ShopOffering
{
    //in this variation, the offering data should be the FLAG we want to trip
    public override void PurchaseItem()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        if (DataManager.instance.money >= o.defaultValue)
        {
            DataManager.instance.SubtractMoney(o.defaultValue);
            DataManager.instance.progressionManager.flags[offeringData] = true;
            DataManager.instance.SendNotification(o.displayName + " Purchased!");
            Destroy(gameObject);
        }
    }
}
