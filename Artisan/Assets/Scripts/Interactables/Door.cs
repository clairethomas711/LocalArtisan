using UnityEngine;

public class Door : Interactable
{
    public override void Interact(FarmManager farm)
    {
        farm.NewDay();
    }
}
