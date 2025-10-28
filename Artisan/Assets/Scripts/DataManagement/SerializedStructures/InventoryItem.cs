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

    public InventoryItem Copy() { return new InventoryItem(id, quantity, customItemData); }
    public void Reset() { id = ""; quantity = 0; customItemData = ""; }

    public void SetCustomData(CustomInventoryItemData data)
    {
        customItemData = JsonUtility.ToJson(data);
    }
    public CustomInventoryItemData GetCustomData()
    {
        if (customItemData != "")
        {
            CustomInventoryItemData data = JsonUtility.FromJson<CustomInventoryItemData>(customItemData);
            return data;
        }
        return null;
    }
}

public class CustomInventoryItemData
{
    public string customName;
    public int additionalValue;
    public Vector3 customColor;
}
