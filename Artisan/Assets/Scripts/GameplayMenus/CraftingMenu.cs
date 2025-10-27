using UnityEngine;
using System.Collections.Generic;

public class CraftingMenu : GameplayMenu
{
    [SerializeField] public MenuMachine machine;
    [SerializeField] public RecipeBook recipeBookDisplay;
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
        //Create a hashset of all the items on our table
        HashSet<string> c = new HashSet<string>();
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i].id != "")
                c.Add(tableSlots[i].id);
        }
        CraftingRecipe validRecipe = FindValidRecipe(c);
        //Are we able to find a valid recipe based on required ingredients?
        if (validRecipe)
        {
            //If it is comparable, we should start producing the item
            machine.PassRecipe(validRecipe);
            //Let's generate the type of product we want to make based on the recipe we give it
            InventoryItem primaryProduct = GenerateRecipeProduct(validRecipe, c);
            //Delete the items that we use for this recipe
            for (int j = 0; j < tableSlots.Count; j++)
            {
                if (tableSlots[j].quantity <= 1)
                    tableSlots[j] = new InventoryItem("", 0);
                else
                    tableSlots[j].quantity -= 1;
            }
            if (validRecipe.product.Count == 1)
                machine.StartProducing(primaryProduct);
            else
                machine.StartProducing(primaryProduct, new InventoryItem(validRecipe.product[1].id, 1));
            UpdateDisplay();
            Close();
        }
        return;
    }

    public CraftingRecipe FindValidRecipe(HashSet<string> tableContents)
    {
        List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
        Dictionary<string, CraftingRecipe>.ValueCollection recipes = DataManager.instance.recipeManifest.Values;
        foreach (CraftingRecipe r in recipes)
        {
            if (machine.recipes.Contains(r.recipeCategory))
                availableRecipes.Add(r);       
        }
        //for each recipe that the machine knows
        for (int i = 0; i < availableRecipes.Count; i++)
        {
            // STEP 1: CHECK THAT WE HAVE ALL OF THE RECIPES STRICT REQUIREMENTS
            CraftingRecipe recipeToCheck = availableRecipes[i];
            //print("Checking recipe: " + availableRecipes[i].recipeDisplayName);
            HashSet<string> itemsUnchecked = new HashSet<string>(tableContents);
            bool hasStrictRequiredItems = true;
            //iterate over every required item in that recipe
            for (int j = 0; j < recipeToCheck.strictRecipeRequirements.Count; j++)
            {
                //try to take that item from the table. If we fail, break.
                if (!AttemptToTakeItem(itemsUnchecked, recipeToCheck.strictRecipeRequirements[j].id))
                {
                    hasStrictRequiredItems = false;
                    break;
                }
            }
            //Did we find all of the STRICT required items? Cool, let's continue
            if (hasStrictRequiredItems)
            {
                // STEP 2: CHECK THAT WE HAVE THE RECIPE'S GENERIC REQUIREMENTS
                bool hasGenericRequiredItems = true;
                for (int j = 0; j < recipeToCheck.genericRecipeRequirements.Count; j++)
                {
                    if (AttemptToTakeTaggedItem(itemsUnchecked, recipeToCheck.genericRecipeRequirements[j]) == "")
                    {
                        hasGenericRequiredItems = false;
                        break;
                    }
                }
                //Did we find all of the GENERIC required items? Cool, then this is the recipe we're looking for
                if (hasGenericRequiredItems)
                {
                    return recipeToCheck;
                }
            }
        }
        //If we get here, then we've found no valid recipe
        DataManager.instance.SendNotification("No valid recipe found");
        return null;
    }

    InventoryItem GenerateRecipeProduct(CraftingRecipe r, HashSet<string> tableContents)
    {
        //If we are calling this, then we KNOW we have the required items. But do we have more than that?
        int numberOfItemsRequired = r.strictRecipeRequirements.Count + r.genericRecipeRequirements.Count;
        //If the number of items on the table is more than the number of items required, then let's check our optional reqs
        if (tableContents.Count > numberOfItemsRequired)
        {
            HashSet<string> itemsUnchecked = new HashSet<string>(tableContents);
            //First remove all of the required items from our set (I HATE THAT WE ITERATE TWICE - CAN WE DO THIS IN FindValidRecipe??)
            for (int i = 0; i < r.strictRecipeRequirements.Count; i++)
            {
                if (!AttemptToTakeItem(itemsUnchecked, r.strictRecipeRequirements[i].id))
                {
                    print("ERROR: Generating a recipe product without required ingredients.");
                    return null;
                }
            }
            for (int i = 0; i < r.genericRecipeRequirements.Count; i++)
            {
                if (AttemptToTakeTaggedItem(itemsUnchecked, r.genericRecipeRequirements[i]) == "")
                {
                    print("ERROR: Generating a recipe product without required ingredients.");
                    return null;
                }
            }
            //Now, with what we have left, make sure that we are allowed to use it. If so, add to the generated item name
            List<string> adjectives = new List<string>();
            for (int i = 0; i < r.strictRecipeOptionals.Count; i++)
            {
                if (AttemptToTakeItem(itemsUnchecked, r.strictRecipeOptionals[i].id))
                {
                    adjectives.Add(r.strictRecipeOptionals[i].displayName);
                }
            }
            for (int i = 0; i < r.genericRecipeOptionals.Count; i++)
            {
                string taggedItem = AttemptToTakeTaggedItem(itemsUnchecked, r.genericRecipeOptionals[i]);
                if (taggedItem != "")
                {
                    adjectives.Add(DataManager.instance.manifest[taggedItem].displayName);
                }
            }
            //Generate the item based on what we found
            adjectives.Sort();
            string customName = "";
            for (int i = 0; i < adjectives.Count; i++)
            {
                customName = customName + adjectives[i] + " ";
            }
            customName += r.product[0].displayName;
            InventoryItem p = new InventoryItem(r.product[0].id, r.quantityProduced);
            print("Generated an item called " + customName);
            return p.GenerateCustomInventoryItem(customName);
        }
        InventoryItem primaryProduct = new InventoryItem(r.product[0].id, r.quantityProduced);
        return primaryProduct;      
    }

    bool AttemptToTakeItem(HashSet<string> itemsUnchecked, string id)
    {
        if (itemsUnchecked.Contains(id))
        {
            //print("Took item: " + id);
            itemsUnchecked.Remove(id);
            return true;
        }
        return false;
    }
    
    string AttemptToTakeTaggedItem(HashSet<string> itemsUnchecked, string tag)
    {
        //check if the hashset contains an item with the given tag. if we find one, remove it and return true. else, return false
        foreach (string s in itemsUnchecked)
        {
            if (DataManager.instance.manifest[s].tag == tag)
            {
                //print("Took tagged item: " + s);
                itemsUnchecked.Remove(s);
                return s;
            }
        }
        return "";   
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
                DataManager.instance.playerInventory.AddInventoryItem(tableSlots[i]);
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

