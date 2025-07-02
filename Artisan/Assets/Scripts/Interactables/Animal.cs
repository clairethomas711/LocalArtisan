using UnityEngine;

public class Animal : Interactable
{
    [SerializeField] private InventoryItem requiredTool;
    public InventoryItem product;
    public bool readyToProduce = false;
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        if (heldItem.name == requiredTool.name && readyToProduce)
        {
            readyToProduce = false;
            farm.playerInventory.AddInventoryItem(product);
        }
    }
}
