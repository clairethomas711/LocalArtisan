using UnityEngine;
using System.Collections.Generic;

public class SellingMenu : GameplayMenu
{
    List<string> acceptedItems = new List<string>();
    List<InventorySlotData> currentInventory = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return currentInventory; }
        set { currentInventory = value; }
    }

    public override void Open(List<InventoryItem> inventory)
    {
        PausePlayer();
        //Open the player's expanded inventory
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);
        //Add blank inventory slots
        for (int i = 0; i < 5; i++)
        {
            inventorySlots[i].currentItem = new InventoryItem("", 0);
        }
        //The given inventory is the items we accept here - populate acceptedItems
        for (int i = 0; i < inventory.Count; i++)
        {
            acceptedItems.Add(inventory[i].id);
        }
        UpdateDisplay();
    }
    public override void Close()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].currentItem != null && inventorySlots[i].currentItem.id != "")
            {
                DataManager.instance.playerInventory.AddInventoryItem(inventorySlots[i].currentItem); //Return unused items to the inventory
            }
        }
        for (int j = 0; j < inventorySlots.Count; j++)
        {
            inventorySlots[j].currentItem = new InventoryItem("", 0);
        }
        UpdateDisplay();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
        UnpausePlayer();    
    }

    public void Sell()
    {
        if (AttemptSale())
        {
            float total = 0;
            //Sell each item
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].currentItem.id != "")
                {
                    DataManager.instance.AddMoney(DataManager.instance.manifest[inventorySlots[i].currentItem.id].defaultValue * inventorySlots[i].currentItem.quantity);
                    total += DataManager.instance.manifest[inventorySlots[i].currentItem.id].defaultValue * inventorySlots[i].currentItem.quantity;
                }
            }
            DataManager.instance.SendNotification("Sold for $" + total);
            //Clear the inventory
            for (int i = 0; i < 5; i++)
            {
                inventorySlots[i].currentItem = new InventoryItem("", 0);
            }
            UpdateDisplay();
        }
        else
        {
            DataManager.instance.SendNotification("Sorry, you can only sell wood here.");
        }
    }

    private bool AttemptSale()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].currentItem.id != "" && !acceptedItems.Contains(inventorySlots[i].currentItem.id))
            {
                return false;
            }
        }
        return true;
    }
}
