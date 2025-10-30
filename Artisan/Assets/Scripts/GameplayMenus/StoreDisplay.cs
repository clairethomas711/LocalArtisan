using UnityEngine;
using System.Collections.Generic;

//Displays a set of items within its inventory, which is saved by the StoreManager. Enables functionality of ShelfSlots
public class StoreDisplay : GameplayMenu
{
    List<InventorySlotData> shelfSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return shelfSlots; }
        set { shelfSlots = value; }
    }

    void Awake()
    {
        //When we first start, make sure the inventory is full of blank items
        for (int i = 0; i < slots.transform.childCount; i++)
            inventorySlots.Add(slots.transform.GetChild(i).gameObject.GetComponent<InventorySlotData>());
    }

    public override void Open(List<InventoryItem> shelfInventory)
    {
        //Pause the player (this is a UI thing, but should probably be moved!)
        PausePlayer();
        //Repopulate this shelf's inventory with data from Store Manager
        for (int i = 0; i < shelfInventory.Count; i++)
            inventorySlots[i].currentItem = shelfInventory[i].Copy();
        //Display the inventory UI
        slots.SetActive(true);
        UpdateDisplay();
    }

    public override void Close()
    {
        UpdateShelfDisplay();
        //Close the inventory UI and release the player
        slots.SetActive(false);
        UnpausePlayer();
    }
    
    public void UpdateShelfDisplay()
    {
        //Update the shelf visuals
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            slots.transform.GetChild(i).GetComponent<ShelfSlotData>().UpdateShelfDisplay();
        }      
    }
}
