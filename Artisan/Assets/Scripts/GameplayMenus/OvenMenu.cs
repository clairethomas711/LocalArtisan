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
        currentPanIndicator.text = "Sheet Pan";      
    }
    public void SwitchPan()
    {
        currentPan++;
        if (currentPan >= unlockedPans.Count)
            currentPan = 0;
        machine.recipes[0] = unlockedPans[currentPan];

        if (currentPan == 0)
            currentPanIndicator.text = "Sheet Pan";
        else if (currentPan == 1)
            currentPanIndicator.text = "Round Pan";

        recipeBookDisplay.CloseRecipeBook();
        recipeBookDisplay.OpenRecipeBook(machine);
    }
}
