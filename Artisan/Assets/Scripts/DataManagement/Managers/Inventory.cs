using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

//Attached to the player, this stores the list of items the player has and controls held item navigation


public class Inventory : MonoBehaviour
{
    public int maxCapacity = 20;
    [HideInInspector] public List<InventoryItem> inventoryList = new List<InventoryItem>();
    [HideInInspector] public bool menuOpen = false;
    public GameObject hotbarPanel;
    public GameObject expandedInventoryPanel;
    public InventoryItem currentSelection;
    int selectedItemLookup = 0;
    private bool inventoryExpanded = false;


    void Start()
    {
        currentSelection = inventoryList[selectedItemLookup];
    }

    public void UpdateInventories()
    {
        if (!inventoryExpanded)
            DisplayHotBar();
        else
            DisplayExpandedInventory();
    }

    void DisplayExpandedInventory()
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            InventorySlotData slot = expandedInventoryPanel.transform.GetChild(i).GetComponent<InventorySlotData>();
            slot.currentItem = inventoryList[i];
            slot.UpdateDisplay();
        }
        currentSelection = inventoryList[selectedItemLookup];
    }

    void DisplayHotBar() //UI Hotbar Display
    {
        ClearHighlight();
        for (int i = 0; i < hotbarPanel.transform.childCount; i++)
        {
            InventorySlotData slot = hotbarPanel.transform.GetChild(i).GetComponent<InventorySlotData>();
            slot.currentItem = inventoryList[i];
            slot.UpdateDisplay();
        }
        currentSelection = inventoryList[selectedItemLookup];
        DisplayHighlight();
    }

    void ClearHighlight() //UI Helper functions
    {
        Transform slot = hotbarPanel.transform.GetChild(selectedItemLookup);
        UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
        s.color = Color.white;
    }

    void DisplayHighlight() //UI Helper functions
    {
        Transform slot = hotbarPanel.transform.GetChild(selectedItemLookup);
        UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
        s.color = Color.green;
    }

    /// INPUT MANAGEMENT ///
    void OnScrollWheel(InputValue scrollValue)
    {
        ClearHighlight();
        //Extract the direction of movement on the hotbar
        Vector2 scrollVector = scrollValue.Get<Vector2>();
        int selectedMovement = (int)scrollVector.y;
        //Move that many places (-1/+1)
        selectedItemLookup = selectedItemLookup - selectedMovement;
        //Loop around if needed
        if (selectedItemLookup >= hotbarPanel.transform.childCount)
            selectedItemLookup = 0;
        else if (selectedItemLookup < 0)
            selectedItemLookup = hotbarPanel.transform.childCount - 1;
        //Update the current item
        currentSelection = inventoryList[selectedItemLookup];
        DisplayHighlight();
    }

    void OnOpenInventory(InputValue ip)
    {
        if (!menuOpen)
        {
            if (!inventoryExpanded)
                OpenExpandedInventory();
            else
                CloseExpandedInventory();
            UpdateInventories();
        }
    }

    public void OnHotbarSelection(InputValue val)
    {
        ClearHighlight();
        selectedItemLookup = (int)val.Get<float>();
        currentSelection = inventoryList[selectedItemLookup];
        DisplayHighlight();
    }

    /// UI EVENT HANDLING ///
    public void ClickHotbarItem(InventorySlotData slot)
    {
        ClearHighlight();
        selectedItemLookup = slot.index;
        DisplayHighlight();
    }

    public void ClickItem(InventorySlotData slot)
    {
        int selectedIndex = slot.index;
        if (inventoryList[selectedIndex].id != "" && inventoryList[selectedIndex].id == DataManager.instance.grab.holding.id) //We are holding the same item - add what we're holding to the stack
        {
            inventoryList[selectedIndex].quantity += DataManager.instance.grab.holding.quantity;
            DataManager.instance.grab.holding = new InventoryItem("", 0);
        }
        else //Otherwise, swap the items
        {
        InventoryItem placeholder = DataManager.instance.grab.holding; //Store the item we're holding
        DataManager.instance.grab.holding = inventoryList[selectedIndex]; //Put the item in this slot into our hand
        inventoryList[selectedIndex] = placeholder; //Put the stored held item in this slot
        }
        UpdateInventories();
    }

    /// PUBLIC FUNCTIONS ///
    public void AddInventoryItem(InventoryItem itemToAdd)
    {
        for (int j = 0; j < inventoryList.Count; j++)
        {
            //If these items are identical, increase quantity
            if (inventoryList[j].id == itemToAdd.id && inventoryList[j].customItemData == itemToAdd.customItemData) 
            {
                inventoryList[j].quantity += itemToAdd.quantity;
                UpdateInventories();
                return;
            }
        }
        //If nothing with the same data found, add
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == "")
            {
                inventoryList[j] = itemToAdd;
                UpdateInventories();
                return;
            }
        }
        print("ERROR: Failed to add to inventory - full!");
    }

    public void RemoveInventoryItem(string i, int quantity = 1)
    {
        if (i == "") { return; }
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == i)
            {
                if (inventoryList[j].quantity - quantity > 0) //Decrease quantity
                {
                    inventoryList[j].quantity -= quantity;
                    UpdateInventories();
                    return;
                }
                else
                {
                    inventoryList[j] = new InventoryItem("", 0);
                    UpdateInventories();
                    return;
                }
            }
        }
    }

    public void OpenExpandedInventory(bool isMenuOpen = false)
    {
        menuOpen = isMenuOpen;
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        inventoryExpanded = true;
        hotbarPanel.SetActive(false);
        expandedInventoryPanel.SetActive(true);
        p.SwitchState(p.busyState);
        UpdateInventories();
        DataManager.instance.PauseGame(true);
    }

    public void CloseExpandedInventory()
    {
        if (menuOpen) { menuOpen = false; }
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        inventoryExpanded = false;
        hotbarPanel.SetActive(true);
        expandedInventoryPanel.SetActive(false);
        p.SwitchState(p.idleState);
        UpdateInventories();
        DataManager.instance.PauseGame(false);
    }
}
