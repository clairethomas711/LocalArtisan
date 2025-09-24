using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string id;
    public int quantity;
    //public int index;

    public InventoryItem(string name, int q)
    {
        id = name;
        quantity = q;
        //index = i;
    }

    public InventoryItem Copy() { return new InventoryItem(id, quantity); }
    public void Reset() { id = ""; quantity = 0;  }
}
