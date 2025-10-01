using UnityEngine;

//A scriptable object for any item that can be added to the player's inventory

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects / Item Data")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public string description;
    public string id;
    public itemType itemType;
    public Sprite sprite;
    public int value;
}

//There NEEDS to be one of these for every item with a different "use" functionality
public enum itemType { Hoe, WateringCan, Pail, Seed, Resource, Artisan, Ingredient, Crop, Axe, Animal };
