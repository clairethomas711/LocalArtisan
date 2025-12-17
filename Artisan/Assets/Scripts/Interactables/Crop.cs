using UnityEngine;
using System.Collections.Generic;
using System.Data;

public class Crop : Interactable
{
    [Header("Crop Settings")]
    [SerializeField] int startingState;
    [SerializeField] string product;
    [SerializeField] List<GameObject> plantStages;
    [Header("Sapling Settings")]
    [SerializeField] string sapling;
    [SerializeField] int minimumMovableState;
    [Header("Regrowth Settings")]
    [SerializeField] bool regrows;
    [SerializeField] int postHarvestState;
    enum PlantState { Unwatered, Watered, Grown }
    PlantState state;
    int growthScore;
    Tile currentTile;

    [System.Serializable]
    private class CropDataPacket
    {
        public PlantState state;
        public int growthScore;
    }
    public override void Initialize(Tile t)
    {
        currentTile = t;
        growthScore = startingState;
        Instantiate(plantStages[growthScore], transform);
    }
    public override string Interact(InventoryItem currentItem)
    {
        //If this tile has a grown plant, we don't need to do any other checks - just harvest
        if (state == PlantState.Grown)
        {
            return UseHarvest();
        }
        //If we aren't holding anything, then we do nothing
        if (currentItem.id == "")
        {
            return "";
        }
        //If we are holding something, then the tile needs to react accordingly
        switch (DataManager.instance.manifest[currentItem.id].itemType)
        {
            case itemType.Hoe:
                return UseHoe();

            case itemType.WateringCan:
                return UseWateringCan();
        }
        return "";
    }

    public override string GetSaveData()
    {
        CropDataPacket saveData = new CropDataPacket();
        saveData.state = state;
        saveData.growthScore = growthScore;
        return JsonUtility.ToJson(saveData);
    }

    public override void SetSaveData(string saveData)
    {
        //Parse the JSON string back into the struct
        CropDataPacket loadedData = JsonUtility.FromJson<CropDataPacket>(saveData);
        state = loadedData.state;
        growthScore = loadedData.growthScore;
        currentTile = transform.parent.gameObject.GetComponent<Tile>();
        //Recreate the plant model
        if (transform.childCount > 0) { Destroy(transform.GetChild(0).gameObject); }
        Instantiate(plantStages[growthScore], transform);
    }

    // LOGIC FOR DETERMINING PLAYER INTENTION - RETURN ANIMATION TRIGGER IF SUCCESSFUL //
    public string UseHoe()
    {
        //If this is a grown, moveable crop and we till it, give the player a sapling and destroy the plant
        if (isMoveableObject && growthScore >= minimumMovableState)
        {
            DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(sapling, 1));
            currentTile.ClearTile();
            Destroy(gameObject);
            return "Hit";
        }
        return "";
    }

    public string UseWateringCan()
    {
        if (state != PlantState.Watered)
        {
            DataManager.instance.progressionManager.QuestSignal(taskType.WaterCrop, product, 1);
        }
        state = PlantState.Watered;
        return currentTile.UseWateringCan();
    }

    public string UseHarvest()
    {
        //Give the player the product
        DataManager.instance.playerInventory.AddInventoryItem(new InventoryItem(product, 1));
        DataManager.instance.progressionManager.QuestSignal(taskType.HarvestCrop, product, 1);
        //If this is a regrowable crop, then reset it's state and display
        if (regrows)
        {
            growthScore = postHarvestState;
            //Update the model
            Destroy(transform.GetChild(0).gameObject);
            Instantiate(plantStages[growthScore], transform);
            state = PlantState.Unwatered;
        }
        //If it isn't destroy the crop
        else
        {
            currentTile.ClearTile();
            Destroy(gameObject);
        }
        return "";
    }
    
    public override void NewDay() //Called within the Data Manager when we process new day updates
    {
        //If we watered the plant, then grow it and clear the water
        if (state == PlantState.Watered)
            growthScore++;
        currentTile.state = Tile.TileState.Tilled;
        state = PlantState.Unwatered;
        //If this growth score is the end of the line, then we are fully grown 
        if (growthScore >= plantStages.Count - 1)
        {
            state = PlantState.Grown;
            growthScore = plantStages.Count - 1;
        }
        //Update the model
        Destroy(transform.GetChild(0).gameObject);
        Instantiate(plantStages[growthScore], transform);
    }
}
