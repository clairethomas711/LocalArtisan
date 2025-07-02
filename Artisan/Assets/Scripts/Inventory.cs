using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

//Attached to the player, this stores the list of items the player has and controls held item navigation

public class Inventory : MonoBehaviour
{
    [SerializeField] List<InventoryItem> startingInventory = new List<InventoryItem>();
    [HideInInspector] public List<InventoryContainer> inventoryList = new List<InventoryContainer>();
    public GameObject inventoryPanel;
    public CursorGrab grab;

    int selectedItemLookup = 0;
    public InventoryContainer currentSelection;
    public InventoryItem EMPTY;

    void Start()
    {
        //Populate the actual inventory
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            inventoryList.Add(inventoryPanel.transform.GetChild(i).GetComponent<InventoryContainer>());
            if (i < startingInventory.Count)
                inventoryList[i].currentItem = startingInventory[i];
            else
                inventoryList[i].currentItem = EMPTY;
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
            UnityEngine.UI.Image s = inventoryList[i].gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = inventoryList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (inventoryList[i].currentItem.name == "") //Don't display empty objects
            {
                text.text = "";
                s.sprite = null;
                continue;
            }
            //Data
            //inventoryList[i].currentItem = inventoryList[i];
            //Image
            s.sprite = inventoryList[i].currentItem.sprite;
            //Quantity
            text.text = inventoryList[i].currentItem.quantity.ToString();
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

    public void ClickItem(InventoryContainer item)
    {
        ClearHighlight();
        InventoryItem placeholder = grab.holding; //Store the item we are holding
        grab.holding = item.currentItem; //Put the item in this slot into our hand
        inventoryList[item.index].currentItem = placeholder; //Put the stored held item into this slot
        currentSelection = inventoryList[item.index];
        selectedItemLookup = item.index;
        DisplayInventory();
    }

        public void AddInventoryItem(InventoryItem i)
    {
        if (i.name == "") { return; }
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].currentItem.name == i.name) //Increase quantity
            {
                inventoryList[j].currentItem.quantity++;
                DisplayInventory();
                return;
            }
        }
        //If nothing with the same name found, add
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].currentItem.name == "")
            {
                inventoryList[j].currentItem = i;
                DisplayInventory();
                return;
            }
        }
        print("ERROR: Failed to add to inventory - full!");
    }

    public void RemoveInventoryItem(InventoryItem i)
    {
        if (i.name == "") { return; }
        for (int j = 0; j < inventoryList.Count; j++)
        {
            if (inventoryList[j].currentItem.name == i.name)
            {
                if (inventoryList[j].currentItem.quantity > 1) //Decrease quantity
                {
                    inventoryList[j].currentItem.quantity--;
                    DisplayInventory();
                    return;
                }
                else
                {
                    inventoryList[j].currentItem = EMPTY;
                    
                    DisplayInventory();
                    return;
                }
            }
        }
    }
}
