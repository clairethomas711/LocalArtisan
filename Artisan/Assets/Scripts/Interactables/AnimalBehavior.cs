using UnityEngine;

public class AnimalBehavior : Interactable
{
    [SerializeField] private ItemData requiredTool;
    [SerializeField] private GameObject indicator;
    public ItemData product;
    public bool readyToProduce = false;
    public override void Interact(InventoryItem heldItem)
    {
        if (heldItem.id == requiredTool.id && readyToProduce)
        {
            //TEMPORARY
            GameObject ready = indicator.transform.GetChild(0).gameObject;
            GameObject done = indicator.transform.GetChild(1).gameObject;
            ready.SetActive(false);
            done.SetActive(true);

            readyToProduce = false;
            DataManager.instance.playerInventory.AddInventoryItem(product.id);
        }
    }

    public void ReadyAnimal()
    {
        //TEMPORARY
        GameObject ready = indicator.transform.GetChild(0).gameObject;
        GameObject done = indicator.transform.GetChild(1).gameObject;
        ready.SetActive(true);
        done.SetActive(false);

        readyToProduce = true;
    }
}
