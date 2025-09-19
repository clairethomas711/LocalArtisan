using UnityEngine;

//for in-game menus where items can be purchased by the player

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects / Shop Data")]
public class ShopData : ItemManifest
{
    public string shopName;
}
