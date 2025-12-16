using UnityEngine;

public class TabUIButton : MonoBehaviour
{
    public GameplayMenu tab; 
    TabGroup tabGroup;
    
    void Start()
    {
        tabGroup = transform.parent.gameObject.GetComponent<TabGroup>();      
    }

    public void OnClickTab()
    {
        tabGroup.SwapTab(this);      
    }
}
