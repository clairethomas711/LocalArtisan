using UnityEngine;

public class Trash : Interactable
{
    [SerializeField] itemType cleaningEquipment;
    public override string Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].itemType == cleaningEquipment)
        {
            DataManager.instance.SubtractStamina(2);
            transform.parent.gameObject.GetComponent<Tile>().state = Tile.TileState.Untilled;
            GetComponent<Animator>().SetTrigger("Destroy");
            return "Hit";
        }
        return "";
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
