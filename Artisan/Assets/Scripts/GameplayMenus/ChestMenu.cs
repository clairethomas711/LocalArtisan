using UnityEngine;
using System.Collections.Generic;

public class ChestMenu : GameplayMenu
{
    List<InventoryItem> chestSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return chestSlots; }
        set { chestSlots = value; }
    }

    public void OpenChest(List<InventoryItem> chestInventory)
    {
        gameObject.SetActive(true);
        int i = 0;
        for (i = 0; i < chestInventory.Count; i++)
        {
            inventorySlots[i] = chestInventory[i];
        }
    }

    public override void Close()
    {
        PlayerStateManager p = farm.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.idleState);
        gameObject.SetActive(false);
    }
}
