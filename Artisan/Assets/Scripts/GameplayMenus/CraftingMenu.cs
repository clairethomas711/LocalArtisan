using UnityEngine;
using System;
using System.Collections.Generic;

public class CraftingMenu : GameplayMenu
{
    [SerializeField] MenuMachine machine;
    List<InventoryItem> tableSlots = new List<InventoryItem>();
    public override List<InventoryItem> inventorySlots
    {
        get { return tableSlots; }
        set { tableSlots = value; }
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
                DataManager.instance.playerInventory.AddInventoryItem(r.product.id); //Once mixed, immediately add to inventory
                for (int j = 0; j < tableSlots.Count; j++)
                {
                    if (tableSlots[j].quantity <= 1)
                        tableSlots[j] = new InventoryItem("", 0);
                    else
                        tableSlots[j].quantity -= 1;
                }
                UpdateDisplay();
                return;
            }
        }
    }

    public override void Open(List<InventoryItem> inventory)
    {
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i] != null && tableSlots[i].id != "")
            {
                DataManager.instance.playerInventory.AddInventoryItem(tableSlots[i].id, tableSlots[i].quantity); //Return unused items to the inventory
            }
        }
        for (int j = 0; j < tableSlots.Count; j++)
        {
            tableSlots[j] = new InventoryItem("", 0);
        }
        UpdateDisplay();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.CloseExpandedInventory();
        gameObject.SetActive(false);
    }
}

