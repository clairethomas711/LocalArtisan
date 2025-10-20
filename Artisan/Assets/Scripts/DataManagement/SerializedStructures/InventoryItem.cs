using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string id;
    public int quantity;
    public string customItemData = "";

    public InventoryItem(string name, int q, string customData = "")
    {
        id = name;
        quantity = q;
        customItemData = customData;
    }

    public InventoryItem GenerateCustomInventoryItem(string customItemData)
    {
        InventoryItem customItem = new InventoryItem(id, quantity);
        customItem.customItemData = customItemData;
        return customItem;
    }
    public InventoryItem Copy() { return new InventoryItem(id, quantity, customItemData); }
    public void Reset() { id = ""; quantity = 0;  }
}
