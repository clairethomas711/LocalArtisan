using UnityEngine;

public class AnimalShopMenu : ShopMenu
{
    public override void PurchaseItem(ItemData itemToPurchase)
    {
        if (DataManager.instance.money >= itemToPurchase.value)
        {
            DataManager.instance.SubtractMoney(itemToPurchase.value);
            BarnManager barnManager = DataManager.instance.barnManager;
            barnManager.AddAnimal(itemToPurchase);
        }
    }
}
