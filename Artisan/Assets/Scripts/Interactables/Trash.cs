using UnityEngine;

public class Trash : Interactable
{
    [SerializeField] itemType cleaningEquipment;
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].itemType == cleaningEquipment)
        {
            GetComponent<Animator>().SetTrigger("Destroy"); //Trigger this object's destruction
            transform.parent.gameObject.GetComponent<Tile>().ClearTile(); //Clear the tile we are on
            return "Hit";
        }
        return "";
    }

    public override string GetSaveData() { return ""; }

    public override void SetSaveData(string saveData) { }

    public override void NewDay() { }

    void Despawn()
    {
        DataManager.instance.playerInventory.AddInventoryItem("res_wood");
        Destroy(gameObject);
    }
}
