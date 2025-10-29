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
    private UnityEngine.UI.Image sprite;
    [HideInInspector]
    public string offeringData; //Data storage

    //Visual control
    public void UpdateOfferingDisplay()
    {
        ItemData o = DataManager.instance.manifest[offeringData];
        itemName.text = o.displayName;
        itemDescription.text = o.description;
        itemPrice.text = o.defaultValue.ToString("n2");
        sprite.sprite = o.primarySprite;
    }

    public abstract void PurchaseItem();
}
