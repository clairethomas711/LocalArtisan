using UnityEngine;

public class Door : Interactable
{
    public override void Interact(InventorySlot heldItem, FarmManager farm)
    {
        farm.NewDay();
    }
}
