using UnityEngine;

public class Bed : Interactable
{
    public override void Interact(InventoryItem heldItem)
    {
        DataManager.instance.NewDay();
    }
}
