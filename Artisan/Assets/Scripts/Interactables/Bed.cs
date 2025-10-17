using UnityEngine;

public class Bed : Interactable
{
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        DataManager.instance.NewDay();
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }
    
    public override void NewDay() { }
}
