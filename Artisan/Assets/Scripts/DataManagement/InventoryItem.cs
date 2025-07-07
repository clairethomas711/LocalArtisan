using UnityEngine;

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
}
