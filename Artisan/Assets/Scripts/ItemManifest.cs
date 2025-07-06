using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemManifest", menuName = "Scriptable Objects / Item Manifest")]
public class ItemManifest : ScriptableObject
{
    public List<ItemData> scriptableItems;
}
