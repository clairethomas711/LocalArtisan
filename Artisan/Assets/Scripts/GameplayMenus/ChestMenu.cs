using UnityEngine;
using System.Collections.Generic;

public class ChestMenu : GameplayMenu
{
    List<InventoryItem> chestSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return chestSlots; }
        set { chestSlots = value; }
    }

    public override void Open(List<InventoryItem> chestInventory)
    {
        Inventory inv = farm.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);
        int i = 0;
        for (i = 0; i < chestInventory.Count; i++)
        {
            inventorySlots[i] = chestInventory[i];
        }
    }

    public override void Close()
    {
        Inventory inv = farm.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
    }
}
