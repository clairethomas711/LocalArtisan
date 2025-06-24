using UnityEngine;

//This class is used to condense the information needed to save and load the tiles within the Farm Manager.

[System.Serializable]
public class Tile
{
    public Vector3 gridLoc;
    public TileBehavior.TileState state;
    public string cropCode;
    public string soilQuality;
    public int plantedDate;
}
