using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//Manages the combined inventories of all store shelves and sells these items. Opens and closes the UI used to alter StoreShelf objects.
public class StoreManager : Interactable
{
    [SerializeField] GameObject closeUI;
    [SerializeField] List<StoreShelf> knownStoreShelves;
    Dictionary<StoreShelf, List<InventoryItem>> storeInventory = new Dictionary<StoreShelf, List<InventoryItem>>(); //Contains all shelf slots
    Dictionary<StoreShelf, List<InventoryItem>> sellableInventory = new Dictionary<StoreShelf, List<InventoryItem>>(); //Contains ONLY shelves with things to sell and those items within
    int timeOfLastSale;

    void Start()
    {
        UpdateInventory();
        DataManager.instance.GameTick.AddListener(StoreTickListener);
        timeOfLastSale = DataManager.instance.totalElapsedMinutes;
    }
    public override void Initialize(Tile t) {}

    public override string Interact(InventoryItem heldItem)
    {
        //Open the player inventory & other visual updates
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        closeUI.SetActive(true);
        //Open each shelf UI individually, passing the known data from the store inventory
        for (int i = 0; i < knownStoreShelves.Count; i++)
        {
            knownStoreShelves[i].Open(storeInventory[knownStoreShelves[i]]);
        }
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }
    
    public void Close()
    {
        //Save any changes to the store inventory
        UpdateInventory();
        //Close each store shelf UI
        for (int i = 0; i < knownStoreShelves.Count; i++)
        {
            knownStoreShelves[i].Close();
        }
        //Close the player inventory and other visual updates
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        closeUI.SetActive(false);
        
    }

    void StoreTickListener()
    {
        if (timeOfLastSale + 60 < DataManager.instance.totalElapsedMinutes)
        {
            SellRandomItem();
            timeOfLastSale = DataManager.instance.totalElapsedMinutes;
        }
    }

    public void UpdateInventory()
    {
        //For each store shelf
        for (int i = 0; i < knownStoreShelves.Count; i++)
        {
            //Clear the list of what we used to know about that shelf
            storeInventory[knownStoreShelves[i]] = new List<InventoryItem>();
            sellableInventory[knownStoreShelves[i]] = new List<InventoryItem>();
            //Replace that list with a new list containing that shelf's current contents
            for (int j = 0; j < knownStoreShelves[i].inventorySlots.Count; j++)
            {
                storeInventory[knownStoreShelves[i]].Add(knownStoreShelves[i].inventorySlots[j]);
                //For the sellable inventory, check if this item is actually sellable
                if (knownStoreShelves[i].inventorySlots[j].id != "")
                {
                    sellableInventory[knownStoreShelves[i]].Add(knownStoreShelves[i].inventorySlots[j]);
                }
            }
            //When we are done going through a shelf, check if any of it was sellable. If not, remove that shelf from the sellable dictionary
            if (sellableInventory[knownStoreShelves[i]].Count <= 0)
            {
                sellableInventory.Remove(knownStoreShelves[i]);        
            }
        }
    }

    void SellRandomItem() //check Time Since Last Sale, if above a threshold then sell a random item.
    {
        if (sellableInventory.Count > 0)
        {
            int selectedShelfInt = Random.Range(0, sellableInventory.Count); //Pick a random shelf from our sellable inventory
            List<InventoryItem> selectedShelfList = sellableInventory.ElementAt(selectedShelfInt).Value; //Grab the list of sellable items at that shelf
            StoreShelf selectedShelf = sellableInventory.ElementAt(selectedShelfInt).Key; //Also save the StoreSelf we picked
            int selectedItemInt = Random.Range(0, selectedShelfList.Count); //Pick a random item from the list of sellable items
            string itemToSell = selectedShelfList[selectedItemInt].id; //That inventory item is the item to sell
            //Sell the item
            DataManager.instance.AddMoney(DataManager.instance.manifest[itemToSell].value);
            DataManager.instance.SendNotification("Sold: " + DataManager.instance.manifest[itemToSell].displayName + " for " + DataManager.instance.manifest[itemToSell].value +"B.");
            //Remove one of those items from that selected StoreShelf object
            for (int i = 0; i < selectedShelf.inventorySlots.Count; i++)
            {
                if (selectedShelf.inventorySlots[i].id == itemToSell) //Once we find the item on that shelf
                {
                    //If there are multiple, remove one. If there is only one, clear it.
                    if (selectedShelf.inventorySlots[i].quantity > 1)
                    {
                        selectedShelf.inventorySlots[i].quantity--;
                    }
                    else
                    {
                        selectedShelf.inventorySlots[i].Reset();
                    }
                    selectedShelf.UpdateDisplay();
                    selectedShelf.UpdateModelDisplay();
                    break;
                }
            }
            UpdateInventory();
        } else
        {
            DataManager.instance.SendNotification("A customer came, but there was nothing to buy.");       
        }
         
    }

    public void SellAllItems()
    {
        /*int profit = 0;
        for (int i = 0; i < knownStoreShelves.Count; i++)
        {
            StoreShelf s = knownStoreShelves[i];
            for (int j = 0; j < s.inventorySlots.Count; j++)
            {
                if (s.inventorySlots[j].id != "")
                    profit += DataManager.instance.manifest[s.inventorySlots[j].id].value;
            }
            //s.shelfInventory.Clear();
            //s.UpdateDisplay();
        }
        DataManager.instance.AddMoney(profit);*/
    }

}
