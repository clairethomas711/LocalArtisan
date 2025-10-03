using UnityEngine;

public class AnimalShopMenu : ShopMenu
{
    public override void PurchaseItem(ItemData itemToPurchase)
    {
        BarnManager barnManager = DataManager.instance.barnManager;
        if (DataManager.instance.money >= itemToPurchase.value && barnManager.AddAnimal(itemToPurchase))
        {
            DataManager.instance.SubtractMoney(itemToPurchase.value);
            DataManager.instance.SendNotification("Cow Purchased!");
        }
    }
}
