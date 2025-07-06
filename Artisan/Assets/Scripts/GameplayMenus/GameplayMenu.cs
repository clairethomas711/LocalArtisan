using UnityEngine;
using System.Collections.Generic;
using TMPro;

public abstract class GameplayMenu : MonoBehaviour
{
    [SerializeField] public FarmManager farm;
    [SerializeField] public GameObject slots;
    public abstract List<InventoryItem> inventorySlots { get; set; }

    void Start()
    {
        for (int i = 0; i < slots.transform.childCount; i++) //Populate our menu storage with empty objects
        {
            inventorySlots.Add(new InventoryItem("", 0));
        }
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Transform slot = slots.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = slot.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (inventorySlots[i].id != "")
            {
                s.sprite = farm.manifest[inventorySlots[i].id].sprite;
                text.text = inventorySlots[i].quantity.ToString();
            }
            else
            {
                s.sprite = null;
                text.text = null;
            }
        }
    }

    public void ClickSlot(InventorySlotData s)
    {
        if (inventorySlots[s.index].id != "" && inventorySlots[s.index].id == farm.grab.holding.id) //We are holding the same item - add what we're holding to the stack
        {
            inventorySlots[s.index].quantity += farm.grab.holding.quantity;
            farm.grab.holding = new InventoryItem("", 0);
        }
        else //Otherwise, swap the items
        {
        InventoryItem placeholder = farm.grab.holding; //Store the item we're holding
        farm.grab.holding = inventorySlots[s.index]; //Put the item in this slot into our hand
        inventorySlots[s.index] = placeholder; //Put the stored held item in this slot
        }
        UpdateDisplay();
    }
    public abstract void Open(List<InventoryItem> inventory = null);
    public abstract void Close();

}
