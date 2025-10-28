using UnityEngine;
using TMPro;

public class InventorySlotData : MonoBehaviour
{
    [HideInInspector] public InventoryItem currentItem;
    public bool required;
    public bool restricted;
    public itemType requiredType = itemType.Ingredient;
    public int index;
    [Header("References")]
    [SerializeField] UnityEngine.UI.Image secondarySpriteDisplay;
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] TextMeshProUGUI quantityDisplay;
    [SerializeField] GameObject artisanIndicator;
    public void UpdateDisplay()
    {
        UnityEngine.UI.Image spriteDisplay = GetComponent<UnityEngine.UI.Image>();
        if (currentItem.id != "")
        {
            //ALWAYS - Display the Sprite, Quantity, and Artisan Indicator
            spriteDisplay.sprite = DataManager.instance.manifest[currentItem.id].primarySprite;
            //Display the quantity (if applicable)
            if (currentItem.quantity > 1)
                quantityDisplay.text = currentItem.quantity.ToString();
            else
                quantityDisplay.text = null;
            //Display artisan indicator (if applicable)
            if (DataManager.instance.manifest[currentItem.id].itemType == itemType.Artisan)
                artisanIndicator.SetActive(true);
            else
                artisanIndicator.SetActive(false);
            //Display the secondary sprite (if applicable)
            if (DataManager.instance.manifest[currentItem.id].decorativeSprite != null)
            {
                secondarySpriteDisplay.gameObject.SetActive(true); 
                secondarySpriteDisplay.sprite = DataManager.instance.manifest[currentItem.id].decorativeSprite;     
            } else
            {
                secondarySpriteDisplay.gameObject.SetActive(false);        
            }
            //SOMETIMES - Determine if we have any custom data
            CustomInventoryItemData data = currentItem.GetCustomData();
            if (data != null)
            {
                spriteDisplay.color = new Color(data.customColor.x, data.customColor.y, data.customColor.z, 1f);
                nameDisplay.text = data.customName + DataManager.instance.manifest[currentItem.id].displayName;
            } else
            {
                spriteDisplay.color = Color.white;
                nameDisplay.text = DataManager.instance.manifest[currentItem.id].displayName;
            }
        }
        else
        {
            spriteDisplay.sprite = null;
            spriteDisplay.color = Color.white;
            secondarySpriteDisplay.gameObject.SetActive(false);   
            nameDisplay.text = null;
            quantityDisplay.text = null;
            artisanIndicator.SetActive(false);
        }
    }
}
