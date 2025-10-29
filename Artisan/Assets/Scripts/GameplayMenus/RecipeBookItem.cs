using UnityEngine;
using TMPro;

public class RecipeBookItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI recipeName;
    [SerializeField] GameObject ingredientsList;
    [SerializeField] GameObject recipeDisplayIngredient;
    public void Initialize(CraftingRecipe recipe)
    {
        bool known = DataManager.instance.progressionManager.isRecipeKnown(recipe.id);
        //Spawn the correct number of recipe ingredients
        for (int i = 0; i < recipe.strictRecipeRequirements.Count; i++)
        {
            GameObject ing = Instantiate(recipeDisplayIngredient, ingredientsList.transform, ingredientsList.transform);
            //If we know the recipe, also replace the sprite for that ingredient
            if (known)
            {
                ing.GetComponent<UnityEngine.UI.Image>().sprite = recipe.strictRecipeRequirements[i].primarySprite;
            }

        }
        for (int i = 0; i < recipe.genericRecipeRequirements.Count; i++)
        {
            GameObject ing = Instantiate(recipeDisplayIngredient, ingredientsList.transform, ingredientsList.transform);
            //If we know the recipe, also replace the sprite for that ingredient
            if (known)
            {
                //THIS IS GROSS - REPLACE THIS LATER WITH A SPRITE FOR EACH GENERIC TAG
                ing.GetComponent<UnityEngine.UI.Image>().sprite = null;
                ing.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = recipe.genericRecipeRequirements[i];

            }

        }
        //If we know the recipe, change the name
        if (known)
        {
            recipeName.text = recipe.recipeDisplayName;
        }
    }
}
