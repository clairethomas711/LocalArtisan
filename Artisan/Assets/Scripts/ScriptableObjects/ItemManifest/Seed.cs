using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Seed", menuName = "Scriptable Objects / Seed")]
public class Seed : ItemData
{
    public List<GameObject> stages;
    public ItemData product;
}
