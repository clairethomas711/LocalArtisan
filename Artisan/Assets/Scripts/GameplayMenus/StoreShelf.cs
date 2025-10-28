using UnityEngine;
using System.Collections.Generic;

//Displays a set of items within its inventory, which is saved by the StoreManager. Enables functionality of ShelfSlots
public class StoreShelf : GameplayMenu
{
    List<InventoryItem> shelfSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return shelfSlots; }
        set { shelfSlots = value; }
    }

    void Awake()
    {
        //When we first start, make sure the inventory is full of blank items
        for (int i = 0; i < slots.transform.childCount; i++)
            inventorySlots.Add(new InventoryItem("", 0));
    }

    public override void Open(List<InventoryItem> shelfInventory)
    {
        //Pause the player (this is a UI thing, but should probably be moved!)
        PausePlayer();
        //Repopulate this shelf's inventory with data from Store Manager
        inventorySlots.Clear();
        for (int i = 0; i < shelfInventory.Count; i++)
            inventorySlots.Add(shelfInventory[i].Copy());
        //Display the inventory UI
        slots.SetActive(true);
        UpdateDisplay();
    }

    public override void Close()
    {
        UpdateModelDisplay();
        //Close the inventory UI and release the player
        slots.SetActive(false);
        UnpausePlayer();
    }
    
    public void UpdateModelDisplay()
    {
        //print("Updating the model display for the " + gameObject.name + " item");
        //Update the shelf visuals
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            slots.transform.GetChild(i).GetComponent<ShelfSlotData>().UpdateShelfDisplay();
        }      
    }
}
