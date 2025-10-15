using UnityEngine;

//The player input manager will raycast for an object that contains a script that is a subtype of Interactable

public abstract class Interactable : MonoBehaviour
{
    public bool isMoveableObject;
    public abstract string Interact(InventoryItem heldItem);
    public bool AttemptToMove(itemType toolUsed)
    {
        if (isMoveableObject && toolUsed == itemType.Axe)
        {
            transform.parent.gameObject.GetComponent<Tile>().Harvest();
            return true;
        }
        else
            return false;
    }
}
