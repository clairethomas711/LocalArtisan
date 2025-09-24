using UnityEngine;
using System.Collections.Generic;

public class ChestMenu : GameplayMenu
{
    public Chest currentChest;
    List<InventoryItem> chestSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return chestSlots; }
        set { chestSlots = value; }
    }

    public override void Open(List<InventoryItem> chestInventory)
    {
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);

        //Copy the chest inventory into the inventory slots
        inventorySlots.Clear();
        for (int i = 0; i < chestInventory.Count; i++)
        {
            inventorySlots.Add(chestInventory[i].Copy());
            //print("Adding chest inventory " + i.ToString() + ": " + chestInventory[i].id);
        }

        UpdateDisplay();
    }

    public override void Close()
    {
        //Copy the current inventory slots into the chest itself
        currentChest.chestInventory.Clear();
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            currentChest.chestInventory.Add(inventorySlots[i].Copy());
            //print("Adding inv inventory " + i.ToString() + ": " + inventorySlots[i].id);
        }

        currentChest = null;
        //Wipe the current inventory slots
        for (int i = 0; i < inventorySlots.Count; i++)
            inventorySlots[i].Reset();

        UpdateDisplay();
        //Close the chest UI
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
    }

    public void ConnectChest(Chest c)
    {
        currentChest = c;
    }
}
