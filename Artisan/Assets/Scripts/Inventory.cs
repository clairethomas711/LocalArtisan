using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//Attached to the player, this stores the list of items the player has and controls held item navigation

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> inventoryList = new List<InventorySlot>();
    public GameObject inventoryPanel;

    int selectedItemLookup = 0;
    public InventorySlot currentItem;

    void Start()
    {
        currentItem = inventoryList[selectedItemLookup];
        DisplayInventory();
        DisplayHighlight();
    }

    void DisplayInventory() //UI Hotbar Display
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            Transform slot = inventoryPanel.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            s.sprite = inventoryList[i].sprite;
        }
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
        currentItem = inventoryList[selectedItemLookup];
        DisplayHighlight();
    }
}
