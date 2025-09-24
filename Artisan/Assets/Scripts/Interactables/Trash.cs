using UnityEngine;

public class Trash : Interactable
{
    [SerializeField] itemType cleaningEquipment;
    public override void Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].itemType == cleaningEquipment)
        {
            DataManager.instance.SubtractStamina(2);
            transform.parent.gameObject.GetComponent<Tile>().state = Tile.TileState.Untilled;
            Destroy(gameObject);
        }
    }
}
