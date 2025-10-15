using UnityEngine;
using System.Collections.Generic;

public class Tile : Interactable
{
    public bool isStatic = false;
    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Grown, Static }; //Switch to using this only for visuals
    [HideInInspector] public TileState state;
    [HideInInspector] public string tileInventory = "";
    
    //Plantable Tile Stuff
    [Header("Visuals")]
    [SerializeField] private Material grass;
    [SerializeField] private Material tilled;
    [SerializeField] private Material watered;
    [SerializeField] GameObject trash;
    [HideInInspector] public int growthScore;
    private string product;
    List<GameObject> plantStages;


    public override string Interact(InventoryItem currentItem)
    {
        //If this tile has a grown plant, we don't need to do any other checks - just harvest
        if (state == TileState.Grown)
        {
            return UseHarvest();
        }
        //If we aren't holding anything, then we need to dig deeper before we react.
        if (currentItem.id == "")
        {
            //if (tileInventory = "") //If the tile doesn't have an inventory, just leave it be
            return ""; 
        }
        //If we are holding something, then the tile needs to react accordingly
        switch (DataManager.instance.manifest[currentItem.id].itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                return UseHoe();

            case itemType.WateringCan:
                return UseWateringCan();

            case itemType.Seed:
                return UseSeed((Seed)DataManager.instance.manifest[currentItem.id]);

            case itemType.Placeable:
                return UsePlaceable(currentItem.id);
        }
        return "";
    }

    // LOGIC FOR DETERMINING PLAYER INTENTION - RETURN ANIMATION TRIGGER IF SUCCESSFUL //
    private string UseHoe()
    {
        //If this tile is static, we can't till it.
        if (isStatic) { return ""; }
        //If this tile isn't empty, we do something different
        if (tileInventory != "") { return ""; }
        //Otherwise, let's attempt to till this tile
        if (Till())
        {
            DataManager.instance.SubtractStamina(2);
            return "Hit";
        }
        return "";
    }

    private string UseWateringCan()
    {
        //If this tile is static, we can't water it
        if (isStatic) { return ""; }
        //I think we should be able to attempt to "water" any plantable tile
        if (Water())
        {
            DataManager.instance.SubtractStamina(2);
            return "Water";
        }
        return "";
    }

    private string UseSeed(Seed currentItem)
    {
        //If this tile is static, we can't plant on it
        if (isStatic) { return ""; }
        //If this tile isn't empty, we shouldn't attempt this
        if (tileInventory != "") { return ""; }
        //If the tile is empty, then we can try planting on it
        if (Plant(currentItem))
            DataManager.instance.SubtractStamina(1);
        return "";
    }

    private string UseHarvest()
    {
        //If this tile is static, we can't plant on it
        if (isStatic) { return ""; }
        if (Harvest())
            DataManager.instance.SubtractStamina(1);
        return "";
    }

    private string UsePlaceable(string currentItem)
    {
        //If this tile has an inventory, we can't place anything on it
        if (tileInventory != "") { return ""; }
        //If we are using a placeable on an empty tile, then we obviously want to place that object
        Place(currentItem);
        return "";
    }
    
    // ONCE WE DETERMINE THE INTENTION, ATTEMPT TO ALTER THE TILE //
    public bool Till()
    {

        if (state == TileState.Untilled || state == TileState.Watered)
        {
            state = TileState.Tilled;
            UpdateVisuals();
            return true;
        }
        else if (state == TileState.Tilled)
        {
            state = TileState.Untilled;
            UpdateVisuals();
            return true;
        }
        return false;
    }

    public bool Water()
    {
        if (state == TileState.Tilled)
        {
            state = TileState.Watered;
            UpdateVisuals();
            return true;
        }
        return false;
    }

    public bool Plant(Seed s)
    {
        if ((state == TileState.Tilled || state == TileState.Watered) && tileInventory == "")
        {
            tileInventory = s.id;
            plantStages = s.stages;
            product = s.product.id;
            growthScore = 0;
            DataManager.instance.playerInventory.RemoveInventoryItem(s.id);
            UpdateVisuals();
            return true;
        }
        return false;
    }

    public bool Harvest() //Used for both crops and resources
    {
        if (state == TileState.Grown)
        {
            DataManager.instance.playerInventory.AddInventoryItem(product);
            state = TileState.Tilled;
        }
        else
        {
            DataManager.instance.playerInventory.AddInventoryItem(tileInventory);
            state = TileState.Untilled;
        }
        tileInventory = "";
        UpdateVisuals();
        return true;
    }

    public bool Place(string currentItem)
    {
        tileInventory = currentItem;
        DataManager.instance.playerInventory.RemoveInventoryItem(currentItem);
        UpdateVisuals();
        return true;
    }

    // HELPER FUNCTIONS FOR ALTERING TILE DATA AND VISUALS //

    public void GenerateNewData()
    {
        tileInventory = "";
        //If this tile is static, then it should have a static visualization. Otherwise, it is untilled.
        if (isStatic)
        {
            state = TileState.Static;       
        } else
        {
            state = TileState.Untilled;
            //Does this tile get a resource on it?
            if (SpawnTrash(0.3f))
            {
                tileInventory = "res_wood"; //Change this later          
            }       
        }
    }
    
    public void GrowPlant() //Called within the Data Manager when we process new day updates
    {
        //If the current tile doesn't have a seed on it, don't worry about this
        if(DataManager.instance.manifest[tileInventory].itemType != itemType.Seed) { return; }
        //If we watered the plant, then grow it and clear the water
        if (state == TileState.Watered)
            growthScore++;
            state = TileState.Tilled;
        //If this growth score is the end of the line, then we are fully grown 
        if (growthScore >= plantStages.Count - 1)
            state = TileState.Grown;
    }
    
    public void UpdateVisuals() //THIS IS A MESS
    {
        //Clear the existing models on this object
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
        //Spawn an object on the tile based on its current inventory
        if (tileInventory != "")
        {
            //If the tile inventory is a seed, then we should spawn the correct growth stage
            if (DataManager.instance.manifest[tileInventory].itemType == itemType.Seed)
            {
                Quaternion properRotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
                Instantiate(plantStages[growthScore], transform.position, properRotation, transform);
            }
            //If the tile inventory is a placeable object, then spawn that
            else if (DataManager.instance.manifest[tileInventory].itemType == itemType.Placeable)
            {
                Placeable p = (Placeable)DataManager.instance.manifest[tileInventory];
                Instantiate(p.prefab, transform.position, transform.rotation, transform);
            }
            //If the tile inventory is a resource, spawn that
            else if (DataManager.instance.manifest[tileInventory].itemType == itemType.Resource)
            {
                Instantiate(trash, transform); //Change this later        
            }
        }
        
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
