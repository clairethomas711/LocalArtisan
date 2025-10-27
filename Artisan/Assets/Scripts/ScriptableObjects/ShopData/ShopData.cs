using UnityEngine;
using System.Collections.Generic;

//for in-game menus where items can be purchased by the player

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects / Shop Data")]
public class ShopData : ScriptableObject
{
    public string shopName;
    public List<ItemData> scriptableItems;
    
}
