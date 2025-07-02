using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Seed", menuName = "Scriptable Objects / Seed")]
public class Seed : InventoryItem
{
    public List<GameObject> stages;
    public InventoryItem product;
}
