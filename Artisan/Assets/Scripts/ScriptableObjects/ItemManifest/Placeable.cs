using UnityEngine;

[CreateAssetMenu(fileName = "Placeable", menuName = "Scriptable Objects / Placeable")]
public class Placeable : ItemData
{
    public Vector2 size = new Vector2(1, 1);
    public GameObject prefab;
}
