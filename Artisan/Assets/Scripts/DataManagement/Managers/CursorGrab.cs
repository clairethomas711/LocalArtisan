using UnityEngine;

public class CursorGrab : InventorySlotData
{
    void Start()
    {
        currentItem = new InventoryItem("", 0);
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
