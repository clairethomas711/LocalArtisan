using UnityEngine;

public class TabGroup : MonoBehaviour
{
    //List<TabButton> tabButtons;
    GameplayMenu currentTab;

    void Start()
    {
        currentTab = transform.GetChild(0).GetComponent<TabUIButton>().tab;
        currentTab.Open();
        DataManager.instance.activeMenu = transform.parent.gameObject.GetComponent<MultiShopMenu>();      
    }
    
    public void SwapTab(TabUIButton tabSelected)
    {
        currentTab.Close();
        currentTab = tabSelected.tab;
        currentTab.Open();
        DataManager.instance.activeMenu = transform.parent.gameObject.GetComponent<MultiShopMenu>();      
    }
}
