using UnityEngine;

public class AnimalBehavior : Interactable
{
    [SerializeField] private ItemData requiredTool;
    public ItemData product;
    public bool readyToProduce = false;
    public override void Interact(InventoryItem heldItem)
    {
        if (heldItem.id == requiredTool.id && readyToProduce)
        {
            readyToProduce = false;
            DataManager.instance.playerInventory.AddInventoryItem(product.id);
        }
    }
}
