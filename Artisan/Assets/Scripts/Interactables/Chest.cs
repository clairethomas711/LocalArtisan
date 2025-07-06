using UnityEngine;
using System.Collections.Generic;

public class Chest : Interactable
{
    [SerializeField] ChestMenu chestMenu;
    List<InventoryItem> chestInventory = new List<InventoryItem>();
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        chestMenu.Open(chestInventory);
    }
}
