using UnityEngine;

//A scriptable object for any item that can be added to the player's inventory

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects / Inventory Item")]
public class InventorySlot : ScriptableObject
{
    public string name;
    public itemType itemType;
    public Sprite sprite;
}

//There NEEDS to be one of these for every item with a different "use" functionality
public enum itemType { Hoe, WateringCan, Seed };
