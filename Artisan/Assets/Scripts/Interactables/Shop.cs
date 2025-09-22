using UnityEngine;
using System.Collections.Generic;

//for in-game menus where items can be purchased by the player

public class Shop : Interactable
{
    [SerializeField] ShopData shopData;
    [SerializeField] ShopMenu shopMenu;
    List<InventoryItem> shopInventory = new List<InventoryItem>();
    public override void Interact(InventoryItem heldItem)
    {
        //This is not a good method and I'm sorry
        for (int i = 0; i < shopData.scriptableItems.Count; i++)
        {
            //The open function needs InventoryItems, so we create those from the ItemData
            shopInventory.Add(new InventoryItem(shopData.scriptableItems[i].id, 1));
        }

        shopMenu.Open(shopInventory);
        shopInventory.Clear();
    }
}
