using UnityEngine;
using System.Collections.Generic;

public class RequiredSubmit : MonoBehaviour
{
    [SerializeField] public GameObject slots;
    List<InventorySlotData> requiredSlots = new List<InventorySlotData>();
    UnityEngine.UI.Button button;

    void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        for (int i = 0; i < slots.transform.childCount; i++)
        {
            InventorySlotData s = slots.transform.GetChild(i).gameObject.GetComponent<InventorySlotData>();
            if (s.required) 
            {
                requiredSlots.Add(s);
            }       
        }
    }
    // Update is called once per frame
    void Update()
    {
        button.interactable = CheckRequiredSlots();
    }

    bool CheckRequiredSlots()
    {
        for (int i = 0; i < requiredSlots.Count; i++)
        {
            if (requiredSlots[i].currentItem.id == "")
            {
                //print("Slot " + requiredSlots[i].index + " has no item.");
                return false;
            }
        }
        return true;
    }
}
