using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public abstract class GameplayMenu : MonoBehaviour
{
    [SerializeField] public GameObject slots;
    public abstract List<InventoryItem> inventorySlots { get; set; }

    public void UpdateDisplay()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            Transform slot = slots.transform.GetChild(i);
            UnityEngine.UI.Image s = slot.gameObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = slot.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (inventorySlots[i].id != "")
            {
                s.sprite = DataManager.instance.manifest[inventorySlots[i].id].sprite;
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
        //FIRST - are we putting the right type of item here?
        if (s.restricted && DataManager.instance.grab.holding.id != "")
        {
            if (DataManager.instance.manifest[DataManager.instance.grab.holding.id].itemType != s.requiredType)
            {
                DataManager.instance.SendNotification("Only items of type " + s.requiredType + " can be put here.");
                return;
            }
        }
        //If so, then let's continue
        if (inventorySlots[s.index].id != "" && inventorySlots[s.index].id == DataManager.instance.grab.holding.id) //We are holding the same item - add what we're holding to the stack
        {
            inventorySlots[s.index].quantity += DataManager.instance.grab.holding.quantity;
            DataManager.instance.grab.holding = new InventoryItem("", 0);
        }
        else //Otherwise, swap the items
        {
            InventoryItem placeholder = DataManager.instance.grab.holding; //Store the item we're holding
            DataManager.instance.grab.holding = inventorySlots[s.index]; //Put the item in this slot into our hand
            inventorySlots[s.index] = placeholder; //Put the stored held item in this slot
            s.currentItem = placeholder;
        }
        UpdateDisplay();
    }

    public void OnCancel(InputAction.CallbackContext ctx) //WHY DOESN'T THIS WORK. FIGURE IT OUT LATER
    {
        print("I hit escape!!");
    }
    public void PausePlayer()
    {
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.busyState);
        DataManager.instance.PauseGame(true);
    }
    public void UnpausePlayer()
    {
        PlayerStateManager p = DataManager.instance.player.GetComponent<PlayerStateManager>();
        p.SwitchState(p.idleState);
        DataManager.instance.PauseGame(false);
    }
    public abstract void Open(List<InventoryItem> inventory = null);
    public abstract void Close();

}
