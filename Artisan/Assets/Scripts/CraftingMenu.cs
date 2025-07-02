using UnityEngine;
using System;
using System.Collections.Generic;

public class CraftingMenu : MonoBehaviour
{
    [SerializeField] FarmManager farm;
    [SerializeField] MenuMachine machine;
    [SerializeField] GameObject slots;
    [SerializeField] CursorGrab grab;
    [SerializeField] InventoryItem EMPTY;
    List<InventoryContainer> tableSlots = new List<InventoryContainer>();

    void Start()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            tableSlots.Add(slots.transform.GetChild(i).GetComponent<InventoryContainer>());
            tableSlots[i].currentItem = EMPTY;
        }
    }

    public void ClickSlot(InventoryContainer craftingTableSlot)
    {
        if (craftingTableSlot.currentItem != EMPTY)
        {
            farm.playerInventory.AddInventoryItem(craftingTableSlot.currentItem);
        }
        craftingTableSlot.currentItem = grab.holding;
        if (grab.holding.quantity > 1)
            grab.holding.quantity--;
        else
            grab.holding = EMPTY;
        UpdateDisplay();
    }

    public void AttempttoCraft() //Called when we click "Submit"
    {
        HashSet<InventoryItem> c = new HashSet<InventoryItem>();
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i].currentItem.name != "")
                c.Add(tableSlots[i].currentItem);
        }
        for (int i = 0; i < machine.recipes.Count; i++) //Look at all the recipes
        {
            CraftingRecipe r = machine.recipes[i];
            HashSet<InventoryItem> required = new HashSet<InventoryItem>(r.recipeRequirements);
            if (required.SetEquals(c)) //We are using a hashset here - better equality checking
            {
                farm.playerInventory.AddInventoryItem(r.product); //Once mixed, immediately add to inventory
                for (int j = 0; j < tableSlots.Count; j++)
                {
                    tableSlots[j].currentItem = EMPTY;
                }
                UpdateDisplay();
                return;
            }
        }
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < tableSlots.Count; i++)
        {
            UnityEngine.UI.Image s = tableSlots[i].gameObject.GetComponent<UnityEngine.UI.Image>();
            if (tableSlots[i] != null && tableSlots[i].currentItem.name != "")
            {
                s.sprite = tableSlots[i].currentItem.sprite;
            }
            else
            {
                s.sprite = null;
            }
        }
    }

    public void CloseCrafting()
    {
        for (int i = 0; i < tableSlots.Count; i++)
        {
            if (tableSlots[i].currentItem != null && tableSlots[i].currentItem.name != "")
            {
                farm.playerInventory.AddInventoryItem(tableSlots[i].currentItem); //Return unused items to the inventory
            }
        }
        for (int j = 0; j < tableSlots.Count; j++)
        {
            tableSlots[j].currentItem = EMPTY;
        }
        UpdateDisplay();
        PlayerStateManager p = farm.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.idleState);
        gameObject.SetActive(false);
    }
}

