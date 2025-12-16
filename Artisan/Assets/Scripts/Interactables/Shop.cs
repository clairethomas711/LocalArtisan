using UnityEngine;
using System.Collections.Generic;

//for in-game menus where items can be purchased by the player

public class Shop : Interactable
{
    
    [SerializeField] GameplayMenu shopMenu;
    
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        shopMenu.Open();
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }
}
