using UnityEngine;
using System.Collections.Generic;

public class ProgressionManager : MonoBehaviour
{
    public Dictionary<string, int> knownSpecializations = new Dictionary<string, int>(); //All the specializations and how many hours we have in each
    public Dictionary<string, int> knownRecipes = new Dictionary<string, int>(); //All recipes we've made and how many times we've made it
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    [System.Serializable]
    private class ProgressionData
    {
        public List<SpecializationProgressionData> knownSpecializations;
        public List<RecipeProgressionData> knownRecipes;
        public List<FlagData> flags;
    }
    [System.Serializable]
    private class RecipeProgressionData
    {
        public string recipeId;
        public int recipeCount;
    }
    [System.Serializable]
    private class SpecializationProgressionData
    {
        public string specializationName;
        public int specializationExp;
    }
    [System.Serializable]
    private class FlagData
    {
        public string flagName;
        public bool flagState;      
    }
    
    public string NewProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        saveData.knownSpecializations = new List<SpecializationProgressionData>();
        saveData.knownRecipes = new List<RecipeProgressionData>();
        saveData.flags = new List<FlagData>();
        return JsonUtility.ToJson(saveData);      
    }

    public string GetProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        //Serialize specialization progression
        saveData.knownSpecializations = new List<SpecializationProgressionData>();
        Dictionary<string, int>.KeyCollection knownSpecializationKeys = knownSpecializations.Keys;
        foreach (string s in knownSpecializationKeys)
        {
            SpecializationProgressionData sp = new SpecializationProgressionData();
            sp.specializationName = s;
            sp.specializationExp = knownSpecializations[s];
            saveData.knownSpecializations.Add(sp);       
        }
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
        //Serialize flags
        saveData.flags = new List<FlagData>();
        Dictionary<string, bool>.KeyCollection flagKeys = flags.Keys;
        foreach (string s in flagKeys)
        {
            FlagData f = new FlagData();
            f.flagName = s;
            f.flagState = flags[s];
            saveData.flags.Add(f);       
        }
        return JsonUtility.ToJson(saveData);
    }
    
    public void SetProgressionData(string saveData)
    {
        ProgressionData loadedData = JsonUtility.FromJson<ProgressionData>(saveData);
        knownSpecializations.Clear();
        knownRecipes.Clear();
        if (saveData == "") { return; }
        //Set specialization data
        for (int i = 0; i < loadedData.knownSpecializations.Count; i++)
        {
            knownSpecializations[loadedData.knownSpecializations[i].specializationName] = loadedData.knownSpecializations[i].specializationExp;       
        }
        //Set recipe data
        for (int i = 0; i < loadedData.knownRecipes.Count; i++)
        {
            knownRecipes[loadedData.knownRecipes[i].recipeId] = loadedData.knownRecipes[i].recipeCount;
        }
        //Set flag data
        for (int i = 0; i < loadedData.flags.Count; i++)
        {
            flags[loadedData.flags[i].flagName] = loadedData.flags[i].flagState;       
        }
    }

    public void SaveMadeRecipe(string r)
    {
        if (!knownSpecializations.ContainsKey("baker")) knownSpecializations["baker"] = 0;
        if (isRecipeKnown(r))
        {
            knownSpecializations["baker"] += DataManager.instance.recipeManifest[r].expGiven;
            knownRecipes[r]++;
        }
        else
        {
            knownSpecializations["baker"] += DataManager.instance.recipeManifest[r].expGiven * 2;
            knownRecipes[r] = 1;
        }
        DataManager.instance.uiManager.UpdateUIVisuals();
    }
    
    public bool isRecipeKnown(string r)
    {
        return knownRecipes.ContainsKey(r);    
    }
}
