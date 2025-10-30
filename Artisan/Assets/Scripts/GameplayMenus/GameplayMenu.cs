using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public abstract class GameplayMenu : MonoBehaviour
{
    [SerializeField] public GameObject slots;
    public abstract List<InventorySlotData> inventorySlots { get; set; }

    public void UpdateDisplay()
    {
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            Transform slot = slots.transform.GetChild(i);
            slot.GetComponent<InventorySlotData>().UpdateDisplay();
        }
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
