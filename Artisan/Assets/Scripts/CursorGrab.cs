using UnityEngine;

public class CursorGrab : MonoBehaviour
{
    public FarmManager farm;
    [HideInInspector] public InventoryItem holding;
    private UnityEngine.UI.Image img;

    void Start()
    {
        img = GetComponent<UnityEngine.UI.Image>();
        holding = new InventoryItem("", 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        if (holding.id != "")
            img.sprite = farm.manifest[holding.id].sprite;
        else
            img.sprite = null;
    }
}
