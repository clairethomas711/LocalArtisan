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
    public void Open(List<InventoryItem> inventory = null)
    {
        PausePlayer();
        gameObject.SetActive(true);
        DataManager.instance.activeMenu = this;
        CustomOpen(inventory);      
    }
    public void Close()
    {
        DataManager.instance.activeMenu = null;
        CustomClose(); 
        gameObject.SetActive(false);
        UnpausePlayer();     
    }
    public abstract void CustomOpen(List<InventoryItem> inventory = null);
    public abstract void CustomClose();

}
