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
        CustomInventoryItemData data;
        if (customItemData != "")
        {
            data = JsonUtility.FromJson<CustomInventoryItemData>(customItemData);
        }
        else
        {
            data = new CustomInventoryItemData();
            data.name = DataManager.instance.manifest[id].displayName;
            data.value = DataManager.instance.manifest[id].defaultValue;
            data.customColor = new Vector3(DataManager.instance.manifest[id].color.r,
             DataManager.instance.manifest[id].color.g,
             DataManager.instance.manifest[id].color.b);
        }
        return data;
    }
}

public class CustomInventoryItemData
{
    public string name;
    public int value;
    public Vector3 customColor;
}
