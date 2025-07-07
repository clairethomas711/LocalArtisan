using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [SerializeField] List<ShelfPlacement> allStoreShelves;
    Dictionary<ShelfPlacement, List<InventoryItem>> storeInventory = new Dictionary<ShelfPlacement, List<InventoryItem>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < allStoreShelves.Count; i++)
        {
            storeInventory[allStoreShelves[i]] = allStoreShelves[i].shelfInventory;
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
