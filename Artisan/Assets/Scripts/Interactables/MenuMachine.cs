using UnityEngine;
using System.Collections.Generic;

public class MenuMachine : Machine
{
    [SerializeField] GameplayMenu uiMenu;
    CraftingRecipe currentRecipe;
    List<ItemData> acceptedItems;
    List<ItemData> productedItems;
    public List<recipeCategory> recipes;
    public override List<ItemData> AcceptedItems
    {
        get { return acceptedItems; }
        set { AcceptedItems = value; }
    }
    public override List<ItemData> ProducedItems
    {
        get { return productedItems; }
        set { ProducedItems = value; }
    }

    public override string Interact(InventoryItem heldItem)
    {
        if (heldItem.id != "" && AttemptToMove(DataManager.instance.manifest[heldItem.id].itemType))
            return "Hit";
        if (state == MachineState.ready)
            uiMenu.Open();
        else if (state == MachineState.produced)
            TakeProducedItem();
        return "";
    }

    public void PassRecipe(CraftingRecipe r)
    {
        currentRecipe = r;
    }

    public override int CalculateProcessingTime()
    {
        return currentRecipe.processingTimeInMinutes;
    }

    public override void OnProductCollection()
    {
        DataManager.instance.progressionManager.SaveMadeRecipe(currentRecipe.id);
    }

}
