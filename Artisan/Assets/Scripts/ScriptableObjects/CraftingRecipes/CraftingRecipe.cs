using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects / Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string id;
    public string recipeDisplayName;
    public List<ItemData> recipeRequirements;
    public List<ItemData> product;
    public int quantityProduced = 1;
    public int processingTimeInMinutes;
}
