using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects / Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemData> recipeRequirements;
    public ItemData product;
}
