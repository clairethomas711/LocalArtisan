using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoreManager : MonoBehaviour
{
    [SerializeField] List<ShelfPlacement> allStoreShelves;
    Dictionary<ShelfPlacement, List<InventoryItem>> storeInventory = new Dictionary<ShelfPlacement, List<InventoryItem>>(); //Contains only filled shelf slots??
    //Dictionary<ShelfPlacement, List<InventoryItem>> sellableInventory = new Dictionary<ShelfPlacement, List<InventoryItem>>();
    int hourOfLastSale;

    void Start()
    {
        UpdateInventory();
        DataManager.instance.GameTick.AddListener(StoreTickListener);
        hourOfLastSale = DataManager.instance.gameTime.Hour;
    }

    void StoreTickListener()
    {
        if (hourOfLastSale < DataManager.instance.gameTime.Hour)
        {
            SellRandomItem();
            hourOfLastSale = DataManager.instance.gameTime.Hour;
        }
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < allStoreShelves.Count; i++)
        {
            if (allStoreShelves[i].shelfInventory.Count > 0)
                storeInventory[allStoreShelves[i]] = allStoreShelves[i].shelfInventory;
        }
    }

    void SellRandomItem() //check Time Since Last Sale, if above a threshold then sell a random item.
    {
        if (storeInventory.Count > 0)
        {
            int selectedShelfInt = Random.Range(0, storeInventory.Count);
            List<InventoryItem> selectedShelf = storeInventory.ElementAt(selectedShelfInt).Value;
            int selectedItemInt = Random.Range(0, selectedShelf.Count);
            InventoryItem itemToSell = selectedShelf[selectedItemInt];
            DataManager.instance.AddMoney(DataManager.instance.manifest[itemToSell.id].value);
            print("Sold: " + itemToSell.id);
            selectedShelf.RemoveAt(selectedItemInt);
            storeInventory.ElementAt(selectedShelfInt).Key.UpdateDisplay();
            if (selectedShelf.Count == 0)
            {
                storeInventory.Remove(storeInventory.ElementAt(selectedShelfInt).Key);
            }
        }
    }

    public void SellAllItems()
    {
        int profit = 0;
        for (int i = 0; i < allStoreShelves.Count; i++)
        {
            ShelfPlacement s = allStoreShelves[i];
            for (int j = 0; j < s.shelfInventory.Count; j++)
            {
                profit += DataManager.instance.manifest[s.shelfInventory[j].id].value;
            }
            s.shelfInventory.Clear();
            s.UpdateDisplay();
        }
        DataManager.instance.AddMoney(profit);
    }

}
