using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

//for in-game menus where items can be purchased by the player

public class ShopMenu : GameplayMenu
{
    [SerializeField] GameObject shopOfferingPrefab;
    List<InventorySlotData> listingSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return listingSlots; }
        set { listingSlots = value; }
    }

    List<ItemData> shopManifest = new List<ItemData>();

    public override void Open(List<InventoryItem> shopInventory)
    {
        PausePlayer();
        //Now that the shop is open, we reconstruct the shop manifest using the InventoryItems
        for (int i = 0; i < shopInventory.Count; i++)
        {
            //This gives us the manifest of ItemData that we need
            ItemData manifestGet = DataManager.instance.manifest[shopInventory[i].id];
            //Make sure that we have the required progression levels
            if (manifestGet.requireProgression)
            {
                //If we don't even know this specialization, don't spawn the item
                if (!DataManager.instance.progressionManager.knownSpecializations.ContainsKey(manifestGet.specializationRequired)) { continue; }
                //If we know the specialization but are too low level, don't spawn the item
                if (DataManager.instance.progressionManager.knownSpecializations[manifestGet.specializationRequired] < manifestGet.levelRequired)
                { continue; }        
            }
            shopManifest.Add(manifestGet);
            //Add a listing for this item
            GameObject s = Instantiate(shopOfferingPrefab, slots.transform);
            //Connect the item data
            ShopOffering data = s.GetComponent<ShopOffering>();
            data.offeringData = manifestGet.id;
        }

        UpdateShopDisplay();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            Destroy(slots.transform.GetChild(i).gameObject);
        }
        this.gameObject.SetActive(false);
        UnpausePlayer();
    }

    public void UpdateShopDisplay()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            ShopOffering listing = slots.transform.GetChild(i).GetComponent<ShopOffering>();
            listing.UpdateOfferingDisplay();
        }
    }
}
