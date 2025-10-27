using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Transactions;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] GameObject recipeBookDisplayItem;

    public void OpenRecipeBook(MenuMachine machine)
    {
        List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
        Dictionary<string, CraftingRecipe>.ValueCollection recipes = DataManager.instance.recipeManifest.Values;
        foreach (CraftingRecipe r in recipes)
        {
            if (machine.recipes.Contains(r.recipeCategory))
                availableRecipes.Add(r);       
        }
        for (int i = 0; i < availableRecipes.Count; i++)
        {
            GameObject recipeDisplay = Instantiate(recipeBookDisplayItem, transform);
            if (DataManager.instance.progressionManager.isRecipeKnown(availableRecipes[i].id))
            {
                recipeDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableRecipes[i].recipeDisplayName;
            }
        }
    }
    
    public void CloseRecipeBook()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }      
    }
}
