using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CraftingManifest", menuName = "Scriptable Objects / Crafting Manifest")]
public class CraftingManifest : ScriptableObject
{
    public List<CraftingRecipe> scriptableItems;
}
