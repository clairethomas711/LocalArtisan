using UnityEngine;
using System.Collections.Generic;

public class Chest : Interactable
{
    [SerializeField] ChestMenu chestMenu;
    List<InventoryItem> chestInventory = new List<InventoryItem>();
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        PlayerStateManager p = farm.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.busyState);
        chestMenu.OpenChest(chestInventory);
    }
}
