using UnityEngine;
using System.Collections.Generic;

public class ChestMenu : GameplayMenu
{
    public GameObject slotPrefab;
    string currentChest;
    List<InventorySlotData> chestSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return chestSlots; }
        set { chestSlots = value; }
    }

    public override void CustomOpen(List<InventoryItem> chestInventory)
    {
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);

        //Generate a set of slot data based on inventory from the manifest
        for (int i = 0; i < chestInventory.Count; i++)
        {
            InventorySlotData s = Instantiate(slotPrefab, slots.transform, slots.transform).GetComponent<InventorySlotData>();
            s.currentItem = chestInventory[i].Copy();
            s.index = i;
            inventorySlots.Add(s);
        }

        UpdateDisplay();
    }

    public override void CustomClose()
    {
        //Save the item list into the manifest
        List<InventoryItem> currentChestInventory = DataManager.instance.chestManager.chestManifest[currentChest];
        currentChestInventory.Clear();
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            currentChestInventory.Add(inventorySlots[i].currentItem.Copy());
        }
        //Clear the inventory slots list and destroy gameobjects
        inventorySlots.Clear();
        for (int i = 0; i < slots.transform.childCount; i++) 
        {
            Destroy(slots.transform.GetChild(i).gameObject);
        }
        currentChest = "";
        //Close the chest UI
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
    }

    public void ConnectChest(string c)
    {
        currentChest = c;
    }
}
