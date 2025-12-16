using UnityEngine;
using System.Collections.Generic;

public class MultiShopMenu : GameplayMenu
{
    List<InventorySlotData> shopSlots;
    public override List<InventorySlotData> inventorySlots
    {
        get { return shopSlots; }
        set { shopSlots = value; }
    }
    
    public override void CustomOpen(List<InventoryItem> inventory)
    {
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
    }
    public override void CustomClose()
    {
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
    }
}
