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
    [SerializeField] Animator animatorController;
    [SerializeField] UnityEngine.UI.Image defaultSprite;
    [SerializeField] UnityEngine.UI.Image secondarySpriteDisplay;
    [SerializeField] TextMeshProUGUI nameDisplay;
    [SerializeField] GameObject quantityDisplay;
    [SerializeField] GameObject artisanIndicator;

    public void ClickItem()
    {
        //FIRST - are we putting the right type of item here?
        if (restricted && DataManager.instance.grab.currentItem.id != "")
        {
            if (DataManager.instance.manifest[DataManager.instance.grab.currentItem.id].itemType != requiredType)
            {
                DataManager.instance.SendNotification("Only items of type " + requiredType + " can be put here.");
                return;
            }
        }
        if (currentItem.id != "" && currentItem.Equals(DataManager.instance.grab.currentItem)) //We are holding the same item - add what we're holding to the stack
        {
            currentItem.quantity += DataManager.instance.grab.currentItem.quantity;
            DataManager.instance.grab.currentItem = new InventoryItem("", 0);
        }
        else //Otherwise, swap the items
        {
            InventoryItem placeholder = DataManager.instance.grab.currentItem; //Store the item we're holding
            DataManager.instance.grab.currentItem = currentItem; //Put the item in this slot into our hand
            currentItem = placeholder; //Put the stored held item in this slot
        }
        DataManager.instance.grab.UpdateDisplay();
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        //UnityEngine.UI.Image spriteDisplay = GetComponent<UnityEngine.UI.Image>();
        if (currentItem.id != "")
        {
            defaultSprite.gameObject.SetActive(true); 
            //ALWAYS - Display the Sprite, Quantity, and Artisan Indicator
            defaultSprite.sprite = DataManager.instance.manifest[currentItem.id].primarySprite;
            //Display the quantity (if applicable)
            if (currentItem.quantity > 1) 
            {
                quantityDisplay.SetActive(true);
                quantityDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = currentItem.quantity.ToString();
            }
            else
                quantityDisplay.SetActive(false);
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
            if (data.customColor.x == 0f && data.customColor.y == 0f && data.customColor.z == 0f || DataManager.instance.manifest[currentItem.id].itemType != itemType.Artisan)
                defaultSprite.color = Color.white;
            else
                defaultSprite.color = new Color(data.customColor.x, data.customColor.y, data.customColor.z, 1f);
            nameDisplay.text = data.name + data.value.ToString("n2");
        }
        else
        {
            defaultSprite.gameObject.SetActive(false);  
            secondarySpriteDisplay.gameObject.SetActive(false);   
            nameDisplay.text = null;
            quantityDisplay.SetActive(false);
            artisanIndicator.SetActive(false);
        }
    }

    public void ShowHighlight()
    {
        animatorController.SetBool("isHighlighted", true);
        //UnityEngine.UI.Image s = gameObject.GetComponent<UnityEngine.UI.Image>();
        //s.color = Color.green;
    }

    public void ClearHighlight()
    {
        animatorController.SetBool("isHighlighted", false);
        //UnityEngine.UI.Image s = gameObject.GetComponent<UnityEngine.UI.Image>();
        //s.color = Color.white;
    }
}
