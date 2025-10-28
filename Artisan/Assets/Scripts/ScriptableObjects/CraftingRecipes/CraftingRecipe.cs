using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects / Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string id;
    public string recipeDisplayName;
    public recipeCategory recipeCategory;
    public List<ItemData> strictRecipeRequirements;
    public List<string> genericRecipeRequirements;
    public List<ItemData> strictRecipeOptionals;
    public List<string> genericRecipeOptionals;
    public List<ItemData> product;
    public int quantityProduced = 1;
    public int processingTimeInMinutes;
    public int expGiven;
}

public enum recipeCategory {Mixer, Oven_SheetPan, Oven_RoundPan, Oven_LoafPan, Oven_MuffinPan, Saucepan}
