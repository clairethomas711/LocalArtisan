using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemManifest", menuName = "Scriptable Objects / Item Manifest")]
public class ItemManifest : ScriptableObject
{
    //Split into multiple lists for organization because I was starting to LOSE IT
    //They all go to the same place
    public List<ItemData> scriptableItems;
    public List<ItemData> animalItems;
    public List<ItemData> artisanItems;
    public List<ItemData> placeableItems;
    public List<ItemData> resourceItems;
    public List<ItemData> seedItems;
    public List<ItemData> toolItems;
}
