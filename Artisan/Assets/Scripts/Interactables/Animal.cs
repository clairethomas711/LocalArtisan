using UnityEngine;

public class Animal : Interactable
{
    [SerializeField] private ItemData requiredTool;
    public ItemData product;
    public bool readyToProduce = false;
    public override void Interact(InventoryItem heldItem, FarmManager farm)
    {
        if (heldItem.id == requiredTool.id && readyToProduce)
        {
            readyToProduce = false;
            farm.playerInventory.AddInventoryItem(product.id);
        }
    }
}
