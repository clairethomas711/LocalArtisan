using UnityEngine;

public class AnimalBehavior : Interactable
{
    [SerializeField] private ItemData requiredTool;
    [SerializeField] private GameObject indicator;
    public ItemData product;
    public bool readyToProduce = false;
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        if ((requiredTool == null || heldItem.id == requiredTool.id) && readyToProduce)
        {
            //TEMPORARY
            GameObject ready = indicator.transform.GetChild(0).gameObject;
            GameObject done = indicator.transform.GetChild(1).gameObject;
            ready.SetActive(false);
            done.SetActive(true);

            readyToProduce = false;
            DataManager.instance.playerInventory.AddInventoryItem(product.id);
        }
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }
    public override void NewDay() {}

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
