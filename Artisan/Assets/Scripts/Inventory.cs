using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

//Attached to the player, this stores the list of items the player has and controls held item navigation


public class Inventory : MonoBehaviour
{
    [SerializeField] FarmManager farm;
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] List<string> startingInventory = new List<string>();
    [HideInInspector] public List<InventoryItem> inventoryList = new List<InventoryItem>();
    //[HideInInspector] public List<int> inventoryCount = new List<int>();
    public GameObject inventoryPanel;
    public CursorGrab grab;
    int selectedItemLookup = 0;
    public InventoryItem currentSelection;
    //public InventoryItem EMPTY;

    void Start()
    {
        for (int i = 0; i < maxCapacity; i++)
        {
            inventoryList.Add(new InventoryItem("", 0));
        }
        //Populate the actual inventory
        for (int i = 0; i < startingInventory.Count; i++)
        {
            AddInventoryItem(startingInventory[i]);
        }
        currentSelection = inventoryList[selectedItemLookup];
        DisplayInventory();
        DisplayHighlight();
    }

    public void DisplayInventory() //UI Hotbar Display
    {
        ClearHighlight();
        for (int i = 0; i < inventoryList.Count; i++)
        {
            Transform slot = inventoryPanel.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = slot.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (inventoryList[i].id != "")
            {
                s.sprite = farm.manifest[inventoryList[i].id].sprite;
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
        Transform slot = inventoryPanel.transform.GetChild(selectedItemLookup);
        UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
        s.color = Color.white;
    }

    void DisplayHighlight() //UI Helper functions
    {
        Transform slot = inventoryPanel.transform.GetChild(selectedItemLookup);
        UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
        s.color = Color.green;
    }

    void OnScrollWheel(InputValue scrollValue)
    {
        ClearHighlight();
        //Extract the direction of movement on the hotbar
        Vector2 scrollVector = scrollValue.Get<Vector2>();
        int selectedMovement = (int)scrollVector.y;
        //Move that many places (-1/+1)
        selectedItemLookup = selectedItemLookup - selectedMovement;
        //Loop around if needed
        if (selectedItemLookup >= inventoryList.Count)
            selectedItemLookup = 0;
        else if (selectedItemLookup < 0)
            selectedItemLookup = inventoryList.Count - 1;
        //Update the current item
        currentSelection = inventoryList[selectedItemLookup];
        DisplayHighlight();
    }

    public void ClickItem(InventorySlotData slot)
    {
        ClearHighlight();
        int selectedIndex = slot.index;
        InventoryItem placeholder = grab.holding; //Store the item we're holding
        grab.holding = inventoryList[selectedIndex]; //Put the item in this slot into our hand
        inventoryList[selectedIndex] = placeholder; //Put the stored held item in this slot
        selectedItemLookup = selectedIndex;
        DisplayInventory();
    }

    public void AddInventoryItem(string i)
    {
        if (i == "hi") { return; }
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == i) //Increase quantity
            {
                inventoryList[j].quantity++;
                DisplayInventory();
                return;
            }
        }
        //If nothing with the same name found, add
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == "")
            {
                inventoryList[j] = new InventoryItem(i, 1);
                DisplayInventory();
                return;
            }
        }
        print("ERROR: Failed to add to inventory - full!");
    }

    public void RemoveInventoryItem(string i)
    {
        if (i == "") { return; }
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].id == i)
            {
                if (inventoryList[j].quantity > 1) //Decrease quantity
                {
                    inventoryList[j].quantity--;
                    DisplayInventory();
                    return;
                }
                else
                {
                    inventoryList[j] = new InventoryItem("", 0);
                    DisplayInventory();
                    return;
                }
            }
        }
    }
}
