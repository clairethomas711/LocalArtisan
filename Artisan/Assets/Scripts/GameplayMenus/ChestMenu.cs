using UnityEngine;
using System.Collections.Generic;

public class ChestMenu : GameplayMenu
{
    string currentChest;
    List<InventorySlotData> chestSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return chestSlots; }
        set { chestSlots = value; }
    }

    public override void Open(List<InventoryItem> chestInventory)
    {
        PausePlayer();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);

        //Copy the chest inventory into the inventory slots
        for (int i = 0; i < chestInventory.Count; i++)
        {
            inventorySlots[i].currentItem = chestInventory[i].Copy();
            //print("Adding chest inventory " + i.ToString() + ": " + chestInventory[i].id);
        }

        UpdateDisplay();
    }

    public override void Close()
    {
        List<InventoryItem> currentChestInventory = DataManager.instance.chestManager.chestManifest[currentChest];
        currentChestInventory.Clear();
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            currentChestInventory.Add(inventorySlots[i].currentItem.Copy());
            //print("Adding inv inventory " + i.ToString() + ": " + inventorySlots[i].id);
        }

        currentChest = "";
        //Wipe the current inventory slots
        for (int i = 0; i < inventorySlots.Count; i++)
            inventorySlots[i].currentItem.Reset();

        UpdateDisplay();
        //Close the chest UI
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
        UnpausePlayer();
    }

    public void ConnectChest(string c)
    {
        currentChest = c;
    }
}
