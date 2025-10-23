using UnityEngine;

public class Trash : Interactable
{
    [Header("Trash Settings")]
    [SerializeField] itemType cleaningEquipment;
    [SerializeField] int hitsToBreak = 1;
    [SerializeField] string product;
    [SerializeField] int quantityGiven = 1;
    private int hits = 0;
    public override void Initialize(Tile t)
    {
        RandomizeRotation();      
    }
    public override string Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].itemType == cleaningEquipment)
        {
            hits++;
            if (hits >= hitsToBreak)
            {
                GetComponent<Animator>().SetTrigger("Destroy"); //Trigger this object's destruction
                transform.parent.gameObject.GetComponent<Tile>().ClearTile(); //Clear the tile we are on
            }
            return "Hit";
        }
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }

    void Despawn()
    {
        DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(product, quantityGiven));
        Destroy(gameObject);
    }
}
