using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//Attached to the player, this stores the list of items the player has, controls navigation via the hotbar and handles input for item use.

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> inventoryList = new List<InventorySlot>();
    public GameObject inventoryPanel;

    int selectedItemLookup = 0;
    InventorySlot currentItem;

    ItemUtility util = new ItemUtility();

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

    public void UseObject(GameObject target) //Called by PlayerStateManager when input received - should I just handle the input here?
    {
        switch (currentItem.itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                util.UseHoe(target);
                break;

            case itemType.WateringCan:
                util.UseWateringCan(target);
                break;

            case itemType.Seed:
                util.UseSeed(target, currentItem);
                break;
        }
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
