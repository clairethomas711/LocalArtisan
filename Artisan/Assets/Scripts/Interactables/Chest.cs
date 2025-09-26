using UnityEngine;
using System.Collections.Generic;

public class Chest : Interactable
{
    [SerializeField] ChestMenu chestMenu;
    public int chestCapacity;
    public List<InventoryItem> chestInventory = new List<InventoryItem>();
    public override string Interact(InventoryItem heldItem)
    {
        chestMenu.ConnectChest(this);
        chestMenu.Open(chestInventory);
        return "";
    }

}
