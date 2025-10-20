using UnityEngine;
using System;
using System.Collections.Generic;

public class CraftingMenu : GameplayMenu
{
    [SerializeField] MenuMachine machine;
    [SerializeField] RecipeBook recipeBookDisplay;
    List<InventoryItem> tableSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return tableSlots; }
        set { tableSlots = value; }
    }

    void Start()
    {
        for (int i = 0; i < slots.transform.childCount; i++) //Populate our menu storage with empty objects
        {
            inventorySlots.Add(new InventoryItem("", 0));
        }
    }

    public void AttempttoCraft() //Called when we click "Submit"
    {
        HashSet<string> c = new HashSet<string>();
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i].id != "")
                c.Add(tableSlots[i].id);
        }
        for (int i = 0; i < machine.recipes.Count; i++) //Look at all the recipes
        {
            CraftingRecipe r = machine.recipes[i];
            HashSet<string> required = new HashSet<string>();
            for (int j = 0; j < r.recipeRequirements.Count; j++) { required.Add(r.recipeRequirements[j].id); }
            if (required.SetEquals(c)) //We are using a hashset here - better equality checking
            {
                machine.PassRecipe(r);
                if (r.product.Count == 1)
                    machine.StartProducing(r.product[0], r.quantityProduced);
                else
                    machine.StartProducing(r.product[0], r.quantityProduced, r.product[1]);
                for (int j = 0; j < tableSlots.Count; j++)
                {
                    if (tableSlots[j].quantity <= 1)
                        tableSlots[j] = new InventoryItem("", 0);
                    else
                        tableSlots[j].quantity -= 1;
                }
                UpdateDisplay();
                Close();
                return;
            }
        }
    }

    public override void Open(List<InventoryItem> inventory)
    {
        PausePlayer();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);
        if (recipeBookDisplay) { recipeBookDisplay.OpenRecipeBook(machine); }
    }

    public override void Close()
    {
        //For each item on the crafting table
        for (int i = 0; i < tableSlots.Count; i++)
        {
            //Return unused items to the inventory
            if (tableSlots[i] != null && tableSlots[i].id != "")
            {
                DataManager.instance.playerInventory.AddInventoryItem(tableSlots[i].id, tableSlots[i].quantity);
            }
            //Clear
            tableSlots[i] = new InventoryItem("", 0);
        }
        //Clear the recipe book display
        if (recipeBookDisplay) { recipeBookDisplay.CloseRecipeBook(); }
        UpdateDisplay();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
        UnpausePlayer();
    }
}

