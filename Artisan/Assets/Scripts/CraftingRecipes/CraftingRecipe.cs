using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects / Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public InventoryItem[] recipeRequirements = new InventoryItem[5];
    public InventoryItem product;
}
