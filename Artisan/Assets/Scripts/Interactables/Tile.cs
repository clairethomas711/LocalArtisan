using UnityEngine;
using System.Collections.Generic;

public class Tile : Interactable
{
    public bool isStatic = false;
    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Static }; //Switch to using this only for visuals
    public Vector2 gridLocation;
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
        //tileInventoryId = currentItem; //Save the item ID of what we are placing
        if (PlaceItem(currentItem))
        {
            tileInventory.Initialize(this); //Initialize the new object
            DataManager.instance.playerInventory.RemoveInventoryItem(currentItem); //Remove that item from the inventory
        }
        else
        {
            DataManager.instance.SendNotification("There isn't room for this item!");       
        }
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
        if (tileInventoryId != "" && tileInventoryId != "x")
        {
            PlaceItem();
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
        //Did we visit this tile already while generating data?
        if (tileInventoryId == "x")
        {
            state = TileState.Untilled;
            return;
        }
        //Otherwise, initialize
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
            SpawnTrash(0.2f);
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

    void SpawnTrash(float chance)
    {
        //If we get the initial percentage, then we can move on
        if (chance >= Random.Range(0.0f, 1.0f))
        {
            //What type of trash are we putting here?
            float randomTrashType = Random.Range(0.0f, 1.0f);
            if (randomTrashType > 0.9) //Tree
            {
                PlaceItem("tree_basic");
            }
            else if (randomTrashType > 0.4) //Stone
            {
                PlaceItem("res_stone");
            }
            else //Wood
            {
                PlaceItem("res_wood");
            }
            tileInventory.Initialize(this);
        }
    }

    public bool PlaceItem(string newItem = "")
    {
        if (tileInventoryId == "x") return false; //Make sure we aren't holding something else
        if (newItem == "") { newItem = tileInventoryId; } //If we didn't provide a string, just use the current item
        Placeable p = (Placeable)DataManager.instance.manifest[newItem]; //Grab the correct prefab from the manifest
        //If this is a large item, we need to check / alert surrounding tiles
        if (p.size.x != 1 || p.size.y != 1)
        {
            //Double loop to change all the items
            for (int x = 0; x < p.size.x; x++)
            {
                for (int y = 0; y < p.size.y; y++)
                {
                    Vector2 search = new Vector2(gridLocation.x + x, gridLocation.y + y);
                    //Grab a new tile
                    if (DataManager.instance.tileManifest.ContainsKey(search))
                    {
                        Tile t = DataManager.instance.tileManifest[search];
                        //Change its currentitem
                        if (x == 0 && y == 0) //Don't change the tile we're on
                            continue;
                        if (t.tileInventoryId != "" && t.tileInventoryId != "x")
                        {
                            //print("Too big - this tile contains " + t.tileInventoryId);
                            return false;
                        }
                        t.tileInventoryId = "x";
                    } else
                    {
                        //print("We reached the end of the world");
                        return false;
                    }
                }
            }
        }
        tileInventoryId = newItem; //If we've provided a string, then we should change the inventory
        tileInventory = Instantiate(p.prefab, transform).GetComponent<Interactable>(); //Spawn that prefab and store ref to the component
        if (tileInventory.randomizeRotation)
            tileInventory.RandomizeRotation();
        return true;
    }
}
