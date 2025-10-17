using UnityEngine;
using System.Collections.Generic;

//for in-game menus where items can be purchased by the player

public class Shop : Interactable
{
    [SerializeField] ShopData shopData;
    [SerializeField] GameplayMenu shopMenu;
    List<InventoryItem> shopInventory = new List<InventoryItem>();
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        //This is not a good method and I'm sorry
        for (int i = 0; i < shopData.scriptableItems.Count; i++)
        {
            //The open function needs InventoryItems, so we create those from the ItemData
            shopInventory.Add(new InventoryItem(shopData.scriptableItems[i].id, 1));
        }

        shopMenu.Open(shopInventory);
        shopInventory.Clear();
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }
}
