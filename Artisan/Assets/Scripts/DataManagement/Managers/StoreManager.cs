using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//Manages the combined inventories of all store shelves and sells these items. Opens and closes the UI used to alter StoreShelf objects.
public class StoreManager : Interactable
{
    [SerializeField] GameObject closeUI;
    [SerializeField] List<StoreShelf> knownStoreShelves;
    [SerializeField] int customerSpawnRate = 60;
    [SerializeField] int maxCustomerPurchase = 50;
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
        //CALCULATE THE RATE THAT WE SPAWN CUSTOMERS AT
        if (timeOfLastSale + customerSpawnRate < DataManager.instance.totalElapsedMinutes) //EVENTUALLY - MULTIPLY BY TOURISM RATE
        {
            GenerateCustomer();
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
            knownStoreShelves[i].UpdateModelDisplay();
        }
    }

    int SellRandomItem() //check Time Since Last Sale, if above a threshold then sell a random item.
    {
        if (sellableInventory.Count > 0)
        {
            int selectedDisplayShelfInt = Random.Range(0, sellableInventory.Count); //Pick a random shelf from our sellable inventory
            List<InventoryItem> selectedDisplayShelf = sellableInventory.ElementAt(selectedDisplayShelfInt).Value; //Grab the list of sellable items on that display
            StoreShelf selectedShelf = sellableInventory.ElementAt(selectedDisplayShelfInt).Key; //Also save the StoreSelf we picked
            int selectedItemInt = Random.Range(0, selectedDisplayShelf.Count); //Pick a random item from the display
            InventoryItem itemToSell = selectedDisplayShelf[selectedItemInt]; //That inventory item is the item to sell
            //Sell the item
            int goingPrice = itemToSell.GetCustomData().value;
            DataManager.instance.AddMoney(goingPrice);
            //DataManager.instance.SendNotification("Sold: " + DataManager.instance.manifest[itemToSell].displayName + " for " + DataManager.instance.manifest[itemToSell].value + "B.");
            //Remove one of those items from that selected StoreShelf object
            for (int i = 0; i < selectedShelf.inventorySlots.Count; i++)
            {
                if (selectedShelf.inventorySlots[i].id == itemToSell.id) //Once we find the item on that shelf
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
                    //selectedShelf.UpdateModelDisplay();
                    break;
                }
            }
            UpdateInventory();
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
        int moneySpent = 0;
        while (moneySpent < customerBudget)
        {
            int purchase = SellRandomItem();
            moneySpent += purchase;
            if (purchase == 0) break;
        }
        DataManager.instance.SendNotification("A customer spent " + moneySpent.ToString() + "B at your store.");
    }

}
