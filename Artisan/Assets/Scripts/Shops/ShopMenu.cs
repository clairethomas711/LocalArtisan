using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

//for in-game menus where items can be purchased by the player

public abstract class ShopMenu : GameplayMenu
{
    [SerializeField] GameObject shopOffering;
    List<InventoryItem> listingSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return listingSlots; }
        set { listingSlots = value; }
    }

    List<ItemData> shopManifest = new List<ItemData>();

    public override void Open(List<InventoryItem> shopInventory)
    {
        //Now that the shop is open, we reconstruct the shop manifest using the InventoryItems
        for (int i = 0; i < shopInventory.Count; i++)
        {
            //This gives us the manifest of ItemData that we need
            ItemData manifestGet = DataManager.instance.manifest[shopInventory[i].id];
            shopManifest.Add(manifestGet);
            //Add a listing for this item
            GameObject s = Instantiate(shopOffering, slots.transform);
            //Connect the item data
            ShopOffering data = s.GetComponent<ShopOffering>();
            data.offeringData = manifestGet;
            //Connect button press functionality
            Button b = s.GetComponent<Button>();
            b.onClick.AddListener(() => PurchaseItem(data.offeringData));
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
    }

    public void UpdateShopDisplay()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            ShopOffering listing = slots.transform.GetChild(i).GetComponent<ShopOffering>();
            listing.UpdateOfferingDisplay();
        }
    }

    //Is called by the button when clicked
    public abstract void PurchaseItem(ItemData itemToPurchase);
}
