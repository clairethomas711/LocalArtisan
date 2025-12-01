using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

//Attached to the player, this stores the list of items the player has and controls held item navigation


public class Inventory : MonoBehaviour
{
    public int maxCapacity = 20;
    public int hotbarCapacity = 10;
    //We alter the data in our inventory as if the expanded panel is ALWAYS open
    //If it isn't then the hotbar is just a preview version of that section of inventory
    [HideInInspector] public List<InventorySlotData> inventoryList = new List<InventorySlotData>();
    List<InventorySlotData> hotbar = new List<InventorySlotData>();
    [HideInInspector] public bool menuOpen = false;
    public GameObject hotbarSlotPrefab;
    public GameObject inventorySlotPrefab;
    public GameObject hotbarPanel;
    public GameObject expandedInventoryPanel;
    public InventoryItem currentHotbarSelection;
    int selectedItemLookup = 0;
    private bool inventoryExpanded = false;


    void Awake()
    {
        //Get reference to the inventory slots
        for (int i = 0; i < maxCapacity; i++)
        {
            GameObject invSlot = Instantiate(inventorySlotPrefab, expandedInventoryPanel.transform, expandedInventoryPanel.transform);
            inventoryList.Add(invSlot.GetComponent<InventorySlotData>());
        }
        selectedItemLookup = 0;
    }

    // SAVE DATA STUFF //
    public List<InventoryItem> GetSaveData()
    {
        List<InventoryItem> inv = new List<InventoryItem>();
        for (int i = 0; i < inventoryList.Count; i++)
        {
            inv.Add(inventoryList[i].currentItem);
        }
        return inv;
    }

    public void SetSaveData(List<InventoryItem> inv)
    {
        for (int i = 0; i < inv.Count; i++)
        {
            inventoryList[i].currentItem = inv[i];
            inventoryList[i].UpdateDisplay();
        }
        CloseExpandedInventory();
    }
    
    // HOT BAR DISPLAY //
    void GenerateHotBar()
    {
        for (int i = 0; i < hotbarPanel.transform.childCount; i++)
        {
            GameObject hotbarSlot = hotbarPanel.transform.GetChild(i).gameObject;
            InventorySlotData hotbarInvSlot = hotbarSlot.GetComponent<InventorySlotData>();
            //Clear the existing data
            hotbarInvSlot.currentItem = inventoryList[i].currentItem;;
            hotbarInvSlot.index = i;
            hotbarInvSlot.UpdateDisplay();
            //Add listener - PROBABLY SHOULD GET RID OF THIS EVENTUALLY
            Button b = hotbarSlot.GetComponent<Button>();
            b.onClick.AddListener(() => ClickHotbarItem(hotbarInvSlot));
        }
        currentHotbarSelection = inventoryList[selectedItemLookup].currentItem;
        DisplayHighlight();
    }

    void ClearHighlight()
    {
        InventorySlotData slot = hotbarPanel.transform.GetChild(selectedItemLookup).gameObject.GetComponent<InventorySlotData>();
        slot.ClearHighlight();
    }

    void DisplayHighlight()
    {
        InventorySlotData slot = hotbarPanel.transform.GetChild(selectedItemLookup).gameObject.GetComponent<InventorySlotData>();
        slot.ShowHighlight();
    }

    void OnScrollWheel(InputValue scrollValue) // CONNECTED TO INPUT MANAGER //
    {
        if (inventoryExpanded) return;
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
        currentHotbarSelection = inventoryList[selectedItemLookup].currentItem;
        DisplayHighlight();
    }

    void OnOpenInventory(InputValue ip) // CONNECTED TO INPUT MANAGER //
    {
        if (!menuOpen)
        {
            if (!inventoryExpanded)
                OpenExpandedInventory();
            else
                CloseExpandedInventory();
        }
    }

    public void OnHotbarSelection(InputValue val) // CONNECTED TO INPUT MANAGER //
    {
        ClearHighlight();
        selectedItemLookup = (int)val.Get<float>();
        currentHotbarSelection = inventoryList[selectedItemLookup].currentItem;
        DisplayHighlight();
    }

    public void ClickHotbarItem(InventorySlotData slot)
    {
        ClearHighlight();
        selectedItemLookup = slot.index;
        currentHotbarSelection = inventoryList[selectedItemLookup].currentItem;
        DisplayHighlight();
    }

    /// PUBLIC FUNCTIONS ///
    public void AddInventoryItem(InventoryItem itemToAdd)
    {
        for (int j = 0; j < inventoryList.Count; j++)
        {
            //If these items are identical, increase quantity
            if (inventoryList[j].currentItem.Equals(itemToAdd)) 
            {
                inventoryList[j].currentItem.quantity += itemToAdd.quantity;
                inventoryList[j].UpdateDisplay();
                if (!menuOpen) { GenerateHotBar(); }
                return;
            }
        }
        //If nothing with the same data found, add
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].currentItem.id == "")
            {
                inventoryList[j].currentItem = itemToAdd;
                inventoryList[j].UpdateDisplay();
                if (!menuOpen) { GenerateHotBar(); }
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
            if (inventoryList[j].currentItem.id == i)
            {
                if (inventoryList[j].currentItem.quantity - quantity > 0) //Decrease quantity
                {
                    inventoryList[j].currentItem.quantity -= quantity;
                    inventoryList[j].UpdateDisplay();
                    if (!menuOpen) { GenerateHotBar(); }
                    return;
                }
                else
                {
                    inventoryList[j].currentItem = new InventoryItem("", 0);
                    inventoryList[j].UpdateDisplay();
                    if (!menuOpen) { GenerateHotBar(); }
                    return;
                }
            }
        }
    }

    public void OpenExpandedInventory(bool isMenuOpen = false)
    {
        //Pause the game and player
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.busyState);
        DataManager.instance.PauseGame(true);
        //Change the bools
        menuOpen = isMenuOpen;
        inventoryExpanded = true;
        //Swap the panels
        hotbarPanel.SetActive(false);
        expandedInventoryPanel.SetActive(true);

    }

    public void CloseExpandedInventory()
    {
        //Change the bools
        if (menuOpen) { menuOpen = false; }
        inventoryExpanded = false;
        //Swap over the panels
        hotbarPanel.SetActive(true);
        expandedInventoryPanel.SetActive(false);
        //Generate the hotbar preview
        GenerateHotBar();
        //Free the player
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.idleState);
        DataManager.instance.PauseGame(false);

    }
}
