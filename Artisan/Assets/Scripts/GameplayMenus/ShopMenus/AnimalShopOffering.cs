using UnityEngine;

public class AnimalShopOffering : ShopOffering
{
    public override void PurchaseItem()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        BarnManager barnManager = DataManager.instance.barnManager;
        if (DataManager.instance.money >= o.defaultValue && barnManager.AddAnimal(o))
        {
            DataManager.instance.SubtractMoney(o.defaultValue);
            DataManager.instance.SendNotification(o.displayName + " Purchased!");
        }
    }
}
