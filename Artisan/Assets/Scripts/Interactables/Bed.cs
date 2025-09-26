using UnityEngine;

public class Bed : Interactable
{
    public override string Interact(InventoryItem heldItem)
    {
        DataManager.instance.NewDay();
        return "";
    }
}
