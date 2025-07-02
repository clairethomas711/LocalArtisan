using UnityEngine;

public class CursorGrab : MonoBehaviour
{

    public InventoryItem holding;
    UnityEngine.UI.Image img;

    void Start()
    {
        img = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        if (holding && holding.name != "")
            img.sprite = holding.sprite;
        else
            img.sprite = null;
    }
}
