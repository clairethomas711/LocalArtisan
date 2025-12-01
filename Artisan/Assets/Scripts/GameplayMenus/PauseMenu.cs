using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseMenu : GameplayMenu
{
    [SerializeField] GameObject warningPopup;

    List<InventorySlotData> nonexistantSlots = new List<InventorySlotData>();
    public override List<InventorySlotData> inventorySlots
    {
        get { return nonexistantSlots; }
        set { nonexistantSlots = value; }
    }

    //This is so the game starts a new file with the game PAUSED. Can't figure out how to do it otherwise
    void Start()
    {
        Invoke("CheckIfPaused", 0.5f);    
    }

    void CheckIfPaused()
    {
        if (gameObject.activeSelf)
        {
            DataManager.instance.PauseGame(true);
            PausePlayer();       
        }   
    }

    public void ResetData()
    {
        DataManager.instance.DeleteData();
    }

    public void OpenWarningPopup()
    {
        warningPopup.SetActive(true);      
    }

    public void CloseWarningPopup()
    {
        warningPopup.SetActive(false);      
    }

    public void QuitGame()
    {
        Application.Quit();      
    }


    public override void CustomOpen(List<InventoryItem> inventory = null) {}
    public override void CustomClose() {}
}
