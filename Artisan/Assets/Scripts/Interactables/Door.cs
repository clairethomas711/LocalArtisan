using UnityEngine;

public class Door : Interactable
{
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        farm.NewDay();
    }
}
