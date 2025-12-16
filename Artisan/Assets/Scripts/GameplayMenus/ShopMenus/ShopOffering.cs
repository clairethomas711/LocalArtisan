using UnityEngine;
using TMPro;

public abstract class ShopOffering : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemDescription;
    [SerializeField]
    private TextMeshProUGUI itemPrice;
    [SerializeField]
    private UnityEngine.UI.Image primarySprite;
    [SerializeField]
    private UnityEngine.UI.Image decorativeSprite;
    [HideInInspector]
    public string offeringData; //Data storage

    //Visual control
    public void UpdateOfferingDisplay()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        itemName.text = o.displayName;
        itemDescription.text = o.description;
        itemPrice.text = o.defaultValue.ToString("n2");
        primarySprite.sprite = o.primarySprite;
        if (o.decorativeSprite != null) 
        {
            decorativeSprite.sprite = o.decorativeSprite;
            decorativeSprite.gameObject.SetActive(true);
            if (o.itemType == itemType.Seed)
            {
                decorativeSprite.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);             
            } else
            {
                decorativeSprite.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }     
        }
        else
            decorativeSprite.gameObject.SetActive(false);
    }

    public abstract void PurchaseItem();
}
