using UnityEngine;

public class Trash : Interactable
{
    [SerializeField] itemType cleaningEquipment;
    public override void Interact(InventoryItem heldItem)
    {
        if (DataManager.instance.manifest[heldItem.id].itemType == cleaningEquipment)
        {
            print(heldItem.id);
            transform.parent.gameObject.GetComponent<TileBehavior>().state = TileBehavior.TileState.Untilled;
            Destroy(gameObject);
        }
    }
}
