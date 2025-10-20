using UnityEngine;
using TMPro;
using System.Transactions;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] GameObject recipeBookDisplayItem;

    public void OpenRecipeBook(MenuMachine machine)
    {
        for (int i = 0; i < machine.recipes.Count; i++)
        {
            GameObject recipeDisplay = Instantiate(recipeBookDisplayItem, transform);
            if (DataManager.instance.progressionManager.isRecipeKnown(machine.recipes[i].id))
            {
                recipeDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = machine.recipes[i].recipeDisplayName;
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
