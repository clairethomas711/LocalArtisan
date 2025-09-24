using UnityEngine;
using System.Collections.Generic;

public class Chest : Interactable
{
    [SerializeField] ChestMenu chestMenu;
    public List<InventoryItem> chestInventory = new List<InventoryItem>();
    public override void Interact(InventoryItem heldItem)
    {
        chestMenu.ConnectChest(this);
        chestMenu.Open(chestInventory);
    }

}
