using UnityEngine;

//The player input manager will raycast for an object that contains a script that is a subtype of Interactable

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact(FarmManager farm);
}
