using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//Manages the combined inventories of all store shelves and sells these items. Opens and closes the UI used to alter StoreShelf objects.
public class StoreManager : Interactable
{
    [SerializeField] GameObject closeUI;
    [SerializeField] List<StoreDisplay> knownStoreDisplays;
    [SerializeField] int customerSpawnRate = 60;
    [SerializeField] int maxCustomerPurchase = 50;
    Dictionary<StoreDisplay, List<InventoryItem>> storeInventory = new Dictionary<StoreDisplay, List<InventoryItem>>(); //Contains all shelf slots
    Dictionary<StoreDisplay, List<InventoryItem>> sellableInventory = new Dictionary<StoreDisplay, List<InventoryItem>>(); //Contains ONLY shelves with things to sell and those items within
    public Dictionary<string, int> goodsManifest = new Dictionary<string, int>(); //All items for sale and how many we have (for quests)
    int timeOfLastSale;

    void Start()
    {
        UpdateInventoryData();
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
        for (int i = 0; i < knownStoreDisplays.Count; i++)
        {
            knownStoreDisplays[i].Open(storeInventory[knownStoreDisplays[i]]);
        }
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }
    
    public void Close()
    {
        //Save any changes to the store inventory
        UpdateInventoryData();
        //Close each store shelf UI
        for (int i = 0; i < knownStoreDisplays.Count; i++)
        {
            knownStoreDisplays[i].Close();
        }
        //Close the player inventory and other visual updates
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        closeUI.SetActive(false);
        
    }

    void StoreTickListener()
    {
        //CALCULATE THE RATE THAT WE SPAWN CUSTOMERS AT
        if (timeOfLastSale + customerSpawnRate < DataManager.instance.totalElapsedMinutes) //EVENTUALLY - MULTIPLY BY TOURISM RATE
        {
            GenerateCustomer();
            timeOfLastSale = DataManager.instance.totalElapsedMinutes;
        }
    }

    //Updates the DATA of the inventory of the shop.
    public void UpdateInventoryData()
    {
        goodsManifest = new Dictionary<string, int>();
        //For each store shelf
        for (int i = 0; i < knownStoreDisplays.Count; i++)
        {
            //Clear the list of what we used to know about that shelf
            storeInventory[knownStoreDisplays[i]] = new List<InventoryItem>();
            sellableInventory[knownStoreDisplays[i]] = new List<InventoryItem>();
            //Replace that list with a new list containing that shelf's current contents
            for (int j = 0; j < knownStoreDisplays[i].inventorySlots.Count; j++)
            {
                storeInventory[knownStoreDisplays[i]].Add(knownStoreDisplays[i].inventorySlots[j].currentItem);
                //For the sellable inventory, check if this item is actually sellable
                if (knownStoreDisplays[i].inventorySlots[j].currentItem.id != "")
                {
                    sellableInventory[knownStoreDisplays[i]].Add(knownStoreDisplays[i].inventorySlots[j].currentItem);
                }
                if (goodsManifest.ContainsKey(knownStoreDisplays[i].inventorySlots[j].currentItem.id))
                    goodsManifest[knownStoreDisplays[i].inventorySlots[j].currentItem.id] += knownStoreDisplays[i].inventorySlots[j].currentItem.quantity;
                else
                    goodsManifest[knownStoreDisplays[i].inventorySlots[j].currentItem.id] = knownStoreDisplays[i].inventorySlots[j].currentItem.quantity;
            }
            //When we are done going through a shelf, check if any of it was sellable. If not, remove that shelf from the sellable dictionary
            if (sellableInventory[knownStoreDisplays[i]].Count <= 0)
            {
                sellableInventory.Remove(knownStoreDisplays[i]);
            }
        }
        DataManager.instance.progressionManager.QuestSignal(taskType.StockGood, "", 0);
    }

    //Updates the VISUALS of the inventory of the shop
    public void UpdateInventoryVisuals()
    {
        for (int i = 0; i < knownStoreDisplays.Count; i++)
        {
            knownStoreDisplays[i].UpdateShelfDisplay();   
        }
    }

    float SellRandomItem() //check Time Since Last Sale, if above a threshold then sell a random item.
    {
        if (sellableInventory.Count > 0)
        {
            int selectedDisplayInt = Random.Range(0, sellableInventory.Count); //Pick a random shelf from our sellable inventory
            List<InventoryItem> selectedDisplayInv = sellableInventory.ElementAt(selectedDisplayInt).Value; //Grab the list of sellable items on that display
            StoreDisplay selectedDisplay = sellableInventory.ElementAt(selectedDisplayInt).Key; //Also save the StoreSelf we picked
            int selectedItemInt = Random.Range(0, selectedDisplayInv.Count); //Pick a random item from the display
            InventoryItem itemToSell = selectedDisplayInv[selectedItemInt]; //That inventory item is the item to sell
            //Sell the item
            float goingPrice = itemToSell.GetCustomData().value;
            DataManager.instance.AddMoney(goingPrice);
            //DataManager.instance.SendNotification("Sold: " + DataManager.instance.manifest[itemToSell].displayName + " for " + DataManager.instance.manifest[itemToSell].value + "B.");
            //Remove one of those items from that selected StoreShelf object
            for (int i = 0; i < selectedDisplay.inventorySlots.Count; i++)
            {
                if (selectedDisplay.inventorySlots[i].currentItem.id == itemToSell.id) //Once we find the item on that shelf
                {
                    //If there are multiple, remove one. If there is only one, clear it.
                    if (selectedDisplay.inventorySlots[i].currentItem.quantity > 1)
                    {
                        selectedDisplay.inventorySlots[i].currentItem.quantity--;
                    }
                    else
                    {
                        selectedDisplay.inventorySlots[i].currentItem.Reset();
                    }
                    selectedDisplay.UpdateDisplay();
                    break;
                }
            }
            UpdateInventoryData();
            return goingPrice;
        }
        else
        {
            DataManager.instance.SendNotification("Your store is open, but you are out of products.");
            return 0;
        }

    }
    
    public void GenerateCustomer()
    {
        //Generate the number of buttons that customer is looking to spend
        int customerBudget = Random.Range(1, maxCustomerPurchase); //EVENTUALLY - MULTIPLY BY SHOP REPUTATION
        //Customer buys items until they run out of money
        float moneySpent = 0;
        while (moneySpent < customerBudget)
        {
            float purchase = SellRandomItem();
            moneySpent += purchase;
            if (purchase == 0) break;
        }
        DataManager.instance.SendNotification("A customer spent " + moneySpent.ToString() + "B at your store.");
        UpdateInventoryVisuals();
    }

}
