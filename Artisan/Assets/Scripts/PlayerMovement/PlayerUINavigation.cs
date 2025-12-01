using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUINavigation : MonoBehaviour
{
    public void OnEscape(InputValue escapeValue)
    {
        if (DataManager.instance.activeMenu != null)
        {
            DataManager.instance.activeMenu.Close();       
        }
        else
        {
            DataManager.instance.activeMenu = DataManager.instance.pauseMenu;
            DataManager.instance.pauseMenu.Open();
        }
    }
}
