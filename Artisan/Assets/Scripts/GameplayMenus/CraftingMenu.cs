using UnityEngine;
using System.Collections.Generic;

public class CraftingMenu : GameplayMenu
{
    [SerializeField] public MenuMachine machine;
    [SerializeField] public RecipeBook recipeBookDisplay;
    List<InventorySlotData> tableSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return tableSlots; }
        set { tableSlots = value; }
    }

    void Start()
    {
        for (int i = 0; i < slots.transform.childCount; i++) //Populate our menu storage with empty objects
        {
            inventorySlots.Add(slots.transform.GetChild(i).gameObject.GetComponent<InventorySlotData>());
        }
    }

    public void AttempttoCraft() //Called when we click "Submit"
    {
        //Create a hashset of all the items on our table
        HashSet<InventoryItem> c = new HashSet<InventoryItem>();
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i].currentItem.id != "")
                c.Add(tableSlots[i].currentItem);
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
                if (tableSlots[j].currentItem.quantity <= 1)
                    tableSlots[j].currentItem = new InventoryItem("", 0);
                else
                    tableSlots[j].currentItem.quantity -= 1;
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

    public CraftingRecipe FindValidRecipe(HashSet<InventoryItem> tableContents)
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
            HashSet<InventoryItem> itemsUnchecked = new HashSet<InventoryItem>(tableContents);
            bool hasStrictRequiredItems = true;
            //iterate over every required item in that recipe
            for (int j = 0; j < recipeToCheck.strictRecipeRequirements.Count; j++)
            {
                //try to take that item from the table. If we fail, break.
                InventoryItem check = AttemptToTakeItem(itemsUnchecked, recipeToCheck.strictRecipeRequirements[j].id);
                if (check == null)
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
                    if (AttemptToTakeTaggedItem(itemsUnchecked, recipeToCheck.genericRecipeRequirements[j]) == null)
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

    InventoryItem GenerateRecipeProduct(CraftingRecipe recipe, HashSet<InventoryItem> tableContents)
    {
        //Let's build an item! Assume it has custom data
        CustomInventoryItemData customData = new CustomInventoryItemData();
        customData.name = "";
        customData.value = 0;
        float r = 0; float g = 0; float b = 0; int samples = 0;//Color stuff is complicated
        HashSet<InventoryItem> itemsUnchecked = new HashSet<InventoryItem>(tableContents);
        //First remove all of the required items from our set 
        for (int i = 0; i < recipe.strictRecipeRequirements.Count; i++)
        {
            InventoryItem check = AttemptToTakeItem(itemsUnchecked, recipe.strictRecipeRequirements[i].id);
            if (check == null)
            {
                print("ERROR: Generating a recipe product without required ingredients.");
                return null;
            }
            //Required items don't modify the name or color, but they do mod the value
            customData.value += check.GetCustomData().value;
        }
        //Now, our required generic items. These alter our color and name.
        for (int i = 0; i < recipe.genericRecipeRequirements.Count; i++)
        {
            InventoryItem check = AttemptToTakeTaggedItem(itemsUnchecked, recipe.genericRecipeRequirements[i]);
            if (check == null)
            {
                print("ERROR: Generating a recipe product without required ingredients.");
                return null;
            }
            //Required generics modify everything
            CustomInventoryItemData checkedItemData = check.GetCustomData();
            customData.name += checkedItemData.name + " ";
            customData.value += checkedItemData.value;
            r += checkedItemData.customColor.x;
            g += checkedItemData.customColor.y;
            b += checkedItemData.customColor.z;
            samples++;
        }
        //Now, any optional items
        for (int i = 0; i < recipe.strictRecipeOptionals.Count; i++)
        {
            InventoryItem check = AttemptToTakeItem(itemsUnchecked, recipe.strictRecipeOptionals[i].id);
            if (check != null)
            {
                //We only need to do this if we find something
                CustomInventoryItemData checkedItemData = check.GetCustomData();
                customData.name += checkedItemData.name + " ";
                customData.value += checkedItemData.value;
                r += checkedItemData.customColor.x;
                g += checkedItemData.customColor.y;
                b += checkedItemData.customColor.z;
                samples++;
            }
        }
        for (int i = 0; i < recipe.genericRecipeOptionals.Count; i++)
        {
            InventoryItem check = AttemptToTakeTaggedItem(itemsUnchecked, recipe.genericRecipeOptionals[i]);
            if (check != null)
            {
                CustomInventoryItemData checkedItemData = check.GetCustomData();
                customData.name += checkedItemData.name + " ";
                customData.value += checkedItemData.value;
                r += checkedItemData.customColor.x;
                g += checkedItemData.customColor.y;
                b += checkedItemData.customColor.z;
                samples++;
            }
        }
        //Data Polishing
        customData.name += DataManager.instance.manifest[recipe.product[0].id].displayName; //Add the display name
        customData.value += recipe.processingTimeInMinutes * 0.05f; //Give a bonus value for processing time
        customData.value = customData.value / recipe.quantityProduced; //Divide by the number of items we get
        if (r == 0 && g == 0 && b == 0) customData.customColor = new Vector3(1f,1f,1f);
        else
        {
            r /= samples; g /= samples; b /= samples;
            customData.customColor = new Vector3(r, g, b); //Average the colors
        }
        //Cool, we've assembled everything we need!
        //Do we still have items left on the table? If so, return them to the player
        if (itemsUnchecked.Count > 0)
        {
            foreach (InventoryItem s in itemsUnchecked)
            {
                //Just give them the item back - we clear the table later
                DataManager.instance.playerInventory.AddInventoryItem(s);
            }
        }
        //ALL ITEMS GENERATED LIKE THIS HAVE CUSTOM DATA
        string recipeProductId = recipe.product[0].id;
        int recipeProductQuantity = recipe.quantityProduced;
        string serializedData = JsonUtility.ToJson(customData);
        InventoryItem recipeProduct = new InventoryItem(recipeProductId, recipeProductQuantity, serializedData);
        return recipeProduct;
    }
    
    /*private CustomInventoryItemData GenerateItemFromAddOns(List<InventoryItem> addOns)
    {
        //addOns.Sort();
        CustomInventoryItemData data = new CustomInventoryItemData();
        string customName = "";
        int additionalValue = 0;
        float red = 0; float green = 0; float blue = 0;
        int numberOfSamples = 0;
        for (int i = 0; i < addOns.Count; i++)
        {
            CustomInventoryItemData ingData = addOns[i].GetCustomData();
            if (ingData == null)
            {
                string item = addOns[i].id;
                print("appending " + DataManager.instance.manifest[item].displayName);
                customName += DataManager.instance.manifest[item].displayName + " ";
                additionalValue += DataManager.instance.manifest[item].value;
                red += DataManager.instance.manifest[item].color.r;
                green += DataManager.instance.manifest[item].color.g;
                blue += DataManager.instance.manifest[item].color.b;
            }
            else
            {
                print("appending custom item " + ingData.customName);
                customName += ingData.customName + " ";
                additionalValue += ingData.additionalValue;
                red += ingData.customColor.x;
                green += ingData.customColor.y;
                blue += ingData.customColor.z;
            }
            numberOfSamples++;
        }
        Vector3 newColor = new Vector3(red / numberOfSamples, green / numberOfSamples, blue / numberOfSamples);
        data.customName = customName;
        data.additionalValue = additionalValue;
        data.customColor = newColor;
        return data;
    }*/

    InventoryItem AttemptToTakeItem(HashSet<InventoryItem> itemsUnchecked, string id)
    {
        foreach (InventoryItem i in itemsUnchecked)
        {
            if (i.id == id)
            {
                itemsUnchecked.Remove(i);
                return i;
            }
        }
        return null;
    }
    
    InventoryItem AttemptToTakeTaggedItem(HashSet<InventoryItem> itemsUnchecked, string tag)
    {
        //check if the hashset contains an item with the given tag. if we find one, remove it and return true. else, return false
        foreach (InventoryItem i in itemsUnchecked)
        {
            if (DataManager.instance.manifest[i.id].tag == tag)
            {
                //print("Took tagged item: " + s);
                itemsUnchecked.Remove(i);
                return i;
            }
        }
        return null;   
    }

    public override void Open(List<InventoryItem> inventory)
    {
        PausePlayer();
        Inventory inv = DataManager.instance.player.GetComponent<Inventory>();
        inv.OpenExpandedInventory(true);
        gameObject.SetActive(true);
        if (recipeBookDisplay) { recipeBookDisplay.OpenRecipeBook(machine); }
        UpdateDisplay();
    }

    public override void Close()
    {
        //For each item on the crafting table
        for (int i = 0; i < tableSlots.Count; i++)
        {
            //Return unused items to the inventory
            if (tableSlots[i].currentItem != null && tableSlots[i].currentItem.id != "")
            {
                DataManager.instance.playerInventory.AddInventoryItem(tableSlots[i].currentItem);
            }
            //Clear
            tableSlots[i].currentItem = new InventoryItem("", 0);
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

