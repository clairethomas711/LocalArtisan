using UnityEngine;

//The player input manager will raycast for an object that contains a script that is a subtype of Interactable

public abstract class Interactable : MonoBehaviour
{
    [Header ("Interactable Settings")]
    public bool isMoveableObject;
    public bool randomizeRotation;
    [HideInInspector] public string data;
    public abstract void Initialize(Tile t);
    public abstract string Interact(InventoryItem heldItem);
    public abstract string GetSaveData();
    public abstract void SetSaveData(string saveData);
    public abstract void NewDay();
    public bool AttemptToMove(itemType toolUsed)
    {
        if (isMoveableObject && toolUsed == itemType.Axe)
        {
            //transform.parent.gameObject.GetComponent<Tile>().Harvest();
            return true;
        }
        else
            return false;
    }

    public void RandomizeRotation()
    {
        int newRotation = Random.Range(0, 4);
        transform.RotateAround(transform.position, Vector3.up, 90 * newRotation);
    }
}
