using UnityEngine;
using TMPro;

public class InventorySlotData : MonoBehaviour
{
    [HideInInspector] public InventoryItem currentItem;
    public bool required;
    public bool restricted;
    public itemType requiredType = itemType.Ingredient;
    public int index;
    //public int quantity;

    public void UpdateDisplay()
    {
        UnityEngine.UI.Image s = GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI text = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (currentItem.id != "")
        {
            //Display the item sprite
            s.sprite = DataManager.instance.manifest[currentItem.id].sprite;
            //Display the quantity (if applicable)
            if (currentItem.quantity > 1)
                text.text = currentItem.quantity.ToString();
            else
                text.text = null;
            //Display artisan indicator (if applicable)
            if (DataManager.instance.manifest[currentItem.id].itemType == itemType.Artisan)
                transform.GetChild(1).gameObject.SetActive(true);
            else
                transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            s.sprite = null;
            text.text = null;
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
