using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

//Attached to the player, this stores the list of items the player has and controls held item navigation


public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 20;
    //[SerializeField] List<string> startingInventory = new List<string>();
    [HideInInspector] public List<InventoryItem> inventoryList = new List<InventoryItem>();
    [HideInInspector] public bool menuOpen = false;
    public GameObject hotbarPanel;
    public GameObject expandedInventoryPanel;
    public InventoryItem currentSelection;
    int selectedItemLookup = 0;
    private bool inventoryExpanded = false;


    void Start()
    {
        for (int i = 0; i < maxCapacity; i++)
        {
            inventoryList.Add(new InventoryItem("", 0));
        }
        currentSelection = inventoryList[selectedItemLookup];
        DataManager.instance.AddMoney(10);
    }

    public void UpdateInventories()
    {
        if (!inventoryExpanded)
            DisplayHotBar();
        else
            DisplayExpandedInventory();
    }

    /*public void AddStartingInventory()
    {
        //Populate the actual inventory
        for (int i = 0; i < startingInventory.Count; i++)
        {
            if (DataManager.instance.manifest[startingInventory[i]].itemType == itemType.Seed)
                AddInventoryItem(startingInventory[i], 6);
            else
                AddInventoryItem(startingInventory[i]);
        }
        UpdateInventories();
        DisplayHighlight();
    }*/

    void DisplayExpandedInventory()
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            Transform slot = expandedInventoryPanel.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = slot.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (inventoryList[i].id != "")
            {
                s.sprite = DataManager.instance.manifest[inventoryList[i].id].sprite;
                text.text = inventoryList[i].quantity.ToString();
            }
            else
            {
                s.sprite = null;
                text.text = null;
            }
        }
        currentSelection = inventoryList[selectedItemLookup];
    }

    void DisplayHotBar() //UI Hotbar Display
    {
        ClearHighlight();
        for (int i = 0; i < hotbarPanel.transform.childCount; i++)
        {
            Transform slot = hotbarPanel.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = slot.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (inventoryList[i].id != "")
            {
                s.sprite = DataManager.instance.manifest[inventoryList[i].id].sprite;
                text.text = inventoryList[i].quantity.ToString();
            }
            else
            {
                s.sprite = null;
                text.text = null;
            }
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
    public void AddInventoryItem(string i, int quantity = 1)
    {
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == i) //Increase quantity
            {
                inventoryList[j].quantity += quantity;
                UpdateInventories();
                return;
            }
        }
        //If nothing with the same name found, add
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == "")
            {
                inventoryList[j] = new InventoryItem(i, quantity);
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
    }
}
