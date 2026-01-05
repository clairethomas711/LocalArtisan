using UnityEngine;
using System.Collections.Generic;

public class RequestChestMenu : GameplayMenu
{
    public GameObject slotPrefab;
    string currentChest;
    GameObject currentChestObject;
    List<InventoryItem> currentRequest;
    GameObject objectToBuild;
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

    public void ConnectChest(string c, GameObject chestObject, List<InventoryItem> request, GameObject o)
    {
        currentChest = c;
        currentChestObject = chestObject;
        currentRequest = request;
        objectToBuild = o;
    }

    public void SubmitRequest()
     {
          print("Attempting request submission");
          //For each requested item
          for (int i = 0; i < currentRequest.Count; i++)
          {
               bool itemFound = false;
               //Is this item in the chest based on CURRENT DATA?
               for (int j = 0; j < inventorySlots.Count; j++)
               {
                    if (inventorySlots[j].currentItem.id == currentRequest[i].id)
                    {
                         //Do we have enough?
                         if (inventorySlots[j].currentItem.quantity >= currentRequest[i].quantity)
                         {
                              itemFound = true;
                         }
                    }
               }
               //If we get here, and we still don't have this item in the chest, we've failed
               if (!itemFound)
                    return;
          }
          //IF WE GET HERE, THEN WE HAVE EVERYTHING
          //ADD back into the player inventory anything that isn't part of the request
          for (int i = 0; i < inventorySlots.Count; i++)
          {
               bool isRequestedItem = false;
               for (int j = 0; j < currentRequest.Count; j++)
               {
                    if (inventorySlots[i].currentItem.id == currentRequest[j].id)
                    {
                         isRequestedItem = true;
                         //If this is the thing we need, but we have too much of it, give them the difference
                         if (inventorySlots[i].currentItem.quantity != currentRequest[j].quantity)
                         {
                              InventoryItem item = new InventoryItem(inventorySlots[i].currentItem.id, 
                                   inventorySlots[i].currentItem.quantity - currentRequest[j].quantity);
                              DataManager.instance.playerInventory.AddInventoryItem(item);
                         }
                    }
               }
               //If this is not a requested item, then give it back to them
               if (!isRequestedItem)
               {
                    InventoryItem item = new InventoryItem(inventorySlots[i].currentItem.id, 
                                   inventorySlots[i].currentItem.quantity);
                    DataManager.instance.playerInventory.AddInventoryItem(item);
               }
          }
          //Close the chest to save the data
          Close();
          //Make that machine ready!
          DataManager.instance.progressionManager.QuestSignal(taskType.PlaceItem, objectToBuild.transform.name, 1);
          objectToBuild.GetComponent<Machine>().state = Machine.MachineState.ready;  
          currentChestObject.SetActive(false);        
     }
}
