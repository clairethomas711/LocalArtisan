using UnityEngine;
using System.Collections.Generic;

public class Chest : Interactable
{
    [SerializeField] ChestMenu chestMenu;
    public int chestCapacity;
    public List<InventoryItem> chestInventory = new List<InventoryItem>();
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        if (heldItem.id != "" && AttemptToMove(DataManager.instance.manifest[heldItem.id].itemType))
            return "Hit";
        chestMenu.ConnectChest(this);
        chestMenu.Open(chestInventory);
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }

}
