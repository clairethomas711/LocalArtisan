using UnityEngine;

public class StoreSign : Interactable
{
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem) 
    {
        DataManager.instance.progressionManager.QuestSignal(taskType.InteractItem, "storeSign", 1);
        if (DataManager.instance.store.isOpen)
        {
            DataManager.instance.store.CloseStore();
        } else
        {
            DataManager.instance.store.OpenStore();
        }
        return "";
    }
    public override string GetSaveData() { return ""; }
    public override void SetSaveData(string saveData) {}
    public override void NewDay() {}
}
