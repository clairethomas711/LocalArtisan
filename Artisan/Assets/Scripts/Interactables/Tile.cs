using UnityEngine;
using System.Collections.Generic;

public class Tile : Interactable
{
    public bool isStatic = false;
    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Static }; //Switch to using this only for visuals
    public TileState state;
    [HideInInspector] public string tileInventoryId = "";
    [HideInInspector] public string tileInventoryData = "";
    Interactable tileInventory = null;

    //Plantable Tile Stuff
    [Header("Visuals")]
    [SerializeField] private Material grass;
    [SerializeField] private Material tilled;
    [SerializeField] private Material watered;

    [System.Serializable]
    private class TileDataPacket
    {
        public TileState state;
        public string tileInventoryId;
        public string tileInventoryData;
    }
    public override void Initialize(Tile t) {}
    public override string Interact(InventoryItem currentItem)
    {
        //A tile is only an interactable if it does not currently contain a DIFFERENT interactable.
        if (tileInventoryId != "") { return ""; }
        if (currentItem.id == "") { return ""; } // Stop giving me an error when i click
        //If we are holding something, then the tile needs to react accordingly
        switch (DataManager.instance.manifest[currentItem.id].itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                return UseHoe();

            case itemType.WateringCan:
                return UseWateringCan();

            case itemType.Seed:
                return UsePlaceable(currentItem.id, true);

            case itemType.Placeable:
                return UsePlaceable(currentItem.id);
        }
        return "";
    }

    // LOGIC FOR DETERMINING SUCCESS - RETURN ANIMATION TRIGGER //
    private string UseHoe()
    {
        if (state == TileState.Untilled || state == TileState.Watered)
        {
            state = TileState.Tilled;
            UpdateVisuals();
            return "Hit";
        }
        else if (state == TileState.Tilled)
        {
            state = TileState.Untilled;
            UpdateVisuals();
            return "Hit";
        }
        return "";
    }

    public string UseWateringCan()
    {
        if (state == TileState.Tilled)
        {
            state = TileState.Watered;
            UpdateVisuals();
            return "Water";
        }
        return "";
    }

    private string UsePlaceable(string currentItem, bool seed = false)
    {
        //If we are using a placeable on an empty tile, then we obviously want to place that object
        //If this is a seed item, we need to make sure the tile is tilled
        if (seed)
        {
            if (state == TileState.Untilled)
            {
                return "";
            }
        }
        tileInventoryId = currentItem; //Save the item ID of what we are placing
        Placeable p = (Placeable)DataManager.instance.manifest[tileInventoryId]; //Grab the correct prefab from the manifest
        tileInventory = Instantiate(p.prefab, transform).GetComponent<Interactable>(); //Spawn that prefab and store ref to the component
        tileInventory.Initialize(this); //Initialize the new object
        DataManager.instance.playerInventory.RemoveInventoryItem(currentItem); //Remove that item from the inventory
        UpdateVisuals();
        return "";
    }

    // SAVE DATA //
    public override string GetSaveData()
    {
        //Create a new save data packet
        TileDataPacket saveData = new TileDataPacket();
        //Populate with data we want to save
        saveData.state = state;
        saveData.tileInventoryId = tileInventoryId;
        //Grab additional data if we need it
        if (tileInventory)
        {
            saveData.tileInventoryData = tileInventory.GetSaveData();
        } else
        {
            saveData.tileInventoryData = "";       
        }
        //JSON serialize and return the string
        return JsonUtility.ToJson(saveData);
    }

    public override void SetSaveData(string saveData)
    {
        //Parse the JSON string back into the struct
        TileDataPacket loadedData = JsonUtility.FromJson<TileDataPacket>(saveData);
        //Populate this object with the new data
        state = loadedData.state;
        tileInventoryId = loadedData.tileInventoryId;
        tileInventoryData = loadedData.tileInventoryData;
        //If we have an inventory item, spawn it and pass the data along
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);       
        }
        if (tileInventoryId != "")
        {
            Placeable p = (Placeable)DataManager.instance.manifest[tileInventoryId]; //Grab the correct prefab from the manifest
            tileInventory = Instantiate(p.prefab, transform).GetComponent<Interactable>(); //Spawn that prefab and store ref to the component
            tileInventory.SetSaveData(tileInventoryData);      
        }
        //Update the visuals of this tile based on the new data
        UpdateVisuals();
    }

    // HELPER FUNCTIONS FOR ALTERING TILE DATA AND VISUALS //

    public override void NewDay()
    {
        if (tileInventory)
        {
            tileInventory.NewDay();     
        }      
    }
    public void GenerateNewData()
    {
        tileInventoryId = "";
        //If this tile is static, then it should have a static visualization. Otherwise, it is untilled.
        if (isStatic)
        {
            state = TileState.Static;
        }
        else
        {
            state = TileState.Untilled;
            //Does this tile get a resource on it?
            if (SpawnTrash(0.3f))
            {
                if (SpawnTrash(0.5f))
                    tileInventoryId = "res_wood"; //Change this later
                else
                    tileInventoryId = "res_stone";          
            }
        }
    }

    public void ClearTile()
    {
        if (isStatic) { state = TileState.Static; }
        else {state = TileState.Untilled;}
        tileInventoryId = "";
        tileInventory = null;
        UpdateVisuals();      
    }

    public void UpdateVisuals()
    {   
        //Update the material on the tile based on its state
        MeshRenderer mat = GetComponent<MeshRenderer>();
        if (state == TileState.Untilled)
        {
            mat.material = grass;
        }
        else if (state == TileState.Tilled)
        {

            mat.material = tilled;
        }
        else if (state == TileState.Watered)
        {
            mat.material = watered;
        }
    }

    bool SpawnTrash(float chance)
    {
        if (chance >= Random.Range(0.0f, 1.0f))
        {
            return true;
        }
        return false;
    }
}
