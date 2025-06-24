using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects / Seed")]
public class Seed : InventorySlot
{
    public List<GameObject> stages;
}
