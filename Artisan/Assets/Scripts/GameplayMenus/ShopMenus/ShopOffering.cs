using UnityEngine;
using TMPro;

public class ShopOffering : MonoBehaviour
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
    public ItemData offeringData; //Data storage

    //Visual control
    public void UpdateOfferingDisplay()
    {
        itemName.text = offeringData.displayName;
        itemDescription.text = offeringData.description;
        itemPrice.text = offeringData.value.ToString();
        sprite.sprite = offeringData.sprite;
    }
}
