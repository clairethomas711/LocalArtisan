using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OvenMenu : CraftingMenu
{
    [SerializeField] TextMeshProUGUI currentPanIndicator;
    int currentPan = 0;
    List<recipeCategory> unlockedPans = new List<recipeCategory>();
    public void OnEnable()
    {
        unlockedPans.Clear();
        unlockedPans.Add(recipeCategory.Oven_SheetPan);
        if (DataManager.instance.progressionManager.flags.ContainsKey("hasRoundPan"))
            unlockedPans.Add(recipeCategory.Oven_RoundPan);
        if (DataManager.instance.progressionManager.flags.ContainsKey("hasLoafPan"))
            unlockedPans.Add(recipeCategory.Oven_LoafPan);
        if (DataManager.instance.progressionManager.flags.ContainsKey("hasMuffinPan"))
            unlockedPans.Add(recipeCategory.Oven_MuffinPan);
        currentPanIndicator.text = "Sheet Pan";
        currentPan = 0;
        machine.recipes[0] = unlockedPans[currentPan];      
    }
    public void SwitchPan()
    {
        currentPan++;
        if (currentPan >= unlockedPans.Count)
            currentPan = 0;
        machine.recipes[0] = unlockedPans[currentPan];

        if (machine.recipes[0] == recipeCategory.Oven_SheetPan)
            currentPanIndicator.text = "Sheet Pan";
        else if (machine.recipes[0] == recipeCategory.Oven_RoundPan)
            currentPanIndicator.text = "Round Pan";
        else if (machine.recipes[0] == recipeCategory.Oven_LoafPan)
            currentPanIndicator.text = "Loaf Pan";
        else if (machine.recipes[0] == recipeCategory.Oven_MuffinPan)
            currentPanIndicator.text = "Muffin Pan";

        recipeBookDisplay.CloseRecipeBook();
        recipeBookDisplay.OpenRecipeBook(machine);
    }
}
