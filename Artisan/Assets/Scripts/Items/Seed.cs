using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Seed", menuName = "Scriptable Objects / Seed")]
public class Seed : InventorySlot
{
    public List<GameObject> stages;
    public InventorySlot product;
}
