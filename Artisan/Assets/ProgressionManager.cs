using UnityEngine;
using System.Collections.Generic;

public class ProgressionManager : MonoBehaviour
{
    Dictionary<string, int> knownRecipes = new Dictionary<string, int>(); //All recipes we've made and how many times we've made it

    [System.Serializable]
    private class ProgressionData
    {
        public List<RecipeProgressionData> knownRecipes;
    }
    [System.Serializable]
    private class RecipeProgressionData
    {
        public string recipeId;
        public int recipeCount;
    }
    
    public string NewProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        saveData.knownRecipes = new List<RecipeProgressionData>();
        return JsonUtility.ToJson(saveData);      
    }

    public string GetProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        //Serialize recipe progression
        saveData.knownRecipes = new List<RecipeProgressionData>();
        Dictionary<string, int>.KeyCollection knownRecipeKeys = knownRecipes.Keys;
        foreach (string s in knownRecipeKeys)
        {
            RecipeProgressionData r = new RecipeProgressionData();
            r.recipeId = s;
            r.recipeCount = knownRecipes[s];
            saveData.knownRecipes.Add(r);
        }
        return JsonUtility.ToJson(saveData);
    }
    
    public void SetProgressionData(string saveData)
    {
        ProgressionData loadedData = JsonUtility.FromJson<ProgressionData>(saveData);
        knownRecipes.Clear();
        if (saveData == "") { return; }
        for (int i = 0; i < loadedData.knownRecipes.Count; i++)
        {
            knownRecipes[loadedData.knownRecipes[i].recipeId] = loadedData.knownRecipes[i].recipeCount;       
        }
    }

    public void SaveMadeRecipe(string r)
    {
        if (isRecipeKnown(r))
        {
            knownRecipes[r]++;
        }
        else
        {
            knownRecipes[r] = 1;
        }
    }
    
    public bool isRecipeKnown(string r)
    {
        return knownRecipes.ContainsKey(r);    
    }
}
