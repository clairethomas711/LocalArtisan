using UnityEngine;
using System;
using System.Collections.Generic;

public class Chest : Interactable
{
    public int chestCapacity;
    //ChestMenu chestMenu;
    string uniqueChestId;
    
    public override void Initialize(Tile t)
    {
        //Generate a unique id for this placed chest
        uniqueChestId = Guid.NewGuid().ToString();
        //Populate the inventory with empty inventory objects
        List<InventoryItem> chestInventory = new List<InventoryItem>();
        for (int i = 0; i < chestCapacity; i++)
        {
            chestInventory.Add(new InventoryItem("", 0));
        }
        //Add this to the chest manager manifest
        DataManager.instance.chestManager.chestManifest[uniqueChestId] = chestInventory;
    }
    public override string Interact(InventoryItem heldItem)
    {
        //First, are we trying to break this chest?
        if (heldItem.id != "" && AttemptToMove(DataManager.instance.manifest[heldItem.id].itemType))
        {
            //If so, we need to make sure this chest is empty
            bool empty = true;
            foreach (InventoryItem i in DataManager.instance.chestManager.chestManifest[uniqueChestId])
            {
                if (i.id != "")
                {
                    empty = false;
                    break;
                }
            }
            if (empty)
            {
                Tile t = transform.parent.gameObject.GetComponent<Tile>();
                DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(t.tileInventoryId, 1));
                DataManager.instance.chestManager.chestManifest.Remove(uniqueChestId);
                t.ClearTile();
                Destroy(gameObject);
                return "Hit"; 
            } else
            {
                DataManager.instance.SendNotification("The chest should be empty if you are trying to move it.");
                return ""; 
            }
        }
        //Let the menu know that this is the game object we're interacting with 
        DataManager.instance.chestManager.defaultChestMenu.ConnectChest(uniqueChestId);
        //Grab the chest inventory from the manifest
        List<InventoryItem> chestInventory = DataManager.instance.chestManager.chestManifest[uniqueChestId];
        DataManager.instance.chestManager.defaultChestMenu.Open(chestInventory);
        return "";
    }

    public override string GetSaveData() { return uniqueChestId; }

    public override void SetSaveData(string saveData) { uniqueChestId = saveData; }

    public override void NewDay() { }

}
