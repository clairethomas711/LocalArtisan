using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

//Managing ALL the data - provides millions of references - is a singleton

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("Manager References")]
    [SerializeField] StoreManager store;
    [SerializeField] InterfaceManager uiManager;
    [SerializeField] public GameObject player;
    [SerializeField] public CursorGrab grab;
    [SerializeField] public BarnManager barnManager;
    [SerializeField] public ChestManager chestManager;
    [Header("Data Objects")]
    public ItemManifest itemManifest;
    [Header("Game Settings")]
    [SerializeField] string path;
    [SerializeField] Transform respawnPoint;
    [SerializeField] float maxStamina;
    [Header("Debug Tools")]
    [SerializeField] bool resetInventory;
    [SerializeField] List<string> debugStartingInventory = new List<string>();
    [HideInInspector] public int currentDay;
    [HideInInspector] public int money;
    [HideInInspector] public float stamina;

    [HideInInspector] public Inventory playerInventory;
    [HideInInspector] public Dictionary<string, ItemData> manifest = new Dictionary<string, ItemData>();
    [HideInInspector] public UnityEvent GameTick = new UnityEvent();
    public struct GameTime
    {
        public int Hour;
        public int Min;
        public GameTime(int hour, int min)
        {
            Hour = hour;
            Min = min;
        }
        public override string ToString() => $"{Hour}:{Min.ToString("00")}";
    }
    public GameTime gameTime;
    [HideInInspector] public int totalElapsedMinutes = 0;
    float dayStartTime;
    float gameTimeReal;
    int priorMin = 0;
    bool newHour = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        //BUILD THE DICTIONARY IN THE MANIFEST
        for (int i = 0; i < itemManifest.scriptableItems.Count; i++)
        {
            manifest[itemManifest.scriptableItems[i].id] = itemManifest.scriptableItems[i];
        }
        currentDay = 1;
        stamina = maxStamina;
        playerInventory = player.GetComponent<Inventory>();
        dayStartTime = Time.time;
        gameTimeReal = Time.time;
        gameTime.Hour = 6;

        // DEBUG SHIT //
        if (resetInventory)
        {
            string json = File.ReadAllText(path);
            SaveData farm = JsonUtility.FromJson<SaveData>(json);
            for (int i = 0; i < farm.inv.Count; i++)
            {
                if (i < debugStartingInventory.Count)
                {
                    farm.inv[i].id = debugStartingInventory[i];
                    farm.inv[i].quantity = 1;
                }
                else
                {
                    farm.inv[i].id = "";
                    farm.inv[i].quantity = 0;
                }
            }
            json = JsonUtility.ToJson(farm);
            File.WriteAllText(path, json);
        }

        LoadFarmLayout();
    }

    void Update() 
    {
        GameClockProgress();
    }

    void GameClockProgress() //Game Clock
    {
        //Convert the real time to game time
        gameTimeReal = Time.time;
        float dif = gameTimeReal - dayStartTime;
        gameTime.Min = (int)Mathf.Floor(dif % 60);
        //Is this a new minute? If so, invoke GameTick
        if (priorMin < gameTime.Min)
        {
            priorMin = gameTime.Min;
            totalElapsedMinutes++;
            GameTick.Invoke();
        }
        //Is this a new hour?
        if (gameTime.Min == 0 && newHour)
        {
            priorMin = gameTime.Min;
            GameTick.Invoke();
            gameTime.Hour += 1;
            newHour = false;
        }
        else if (gameTime.Min > 0)
        {
            newHour = true;
        }
        //Visuals Update
        uiManager.UpdateClock();
    }

    public int GameTimeInMinutes()
    {
        return (gameTime.Min + gameTime.Hour * 60);
    }

    public int TotalElapsedGameTime() //THIS DOES NOT ACCOUNT FOR SLEEPING YET - SHOULD ADD A FEW HOURS WHEN YOU DO
    {
        return (totalElapsedMinutes);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        uiManager.UpdateUIVisuals();
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        uiManager.UpdateUIVisuals();
    }

    void RestoreStamina()
    {
        stamina = maxStamina;
        uiManager.UpdateUIVisuals();
    }
    public bool SubtractStamina(float amount)
    {
        if (stamina > 0)
        {
            stamina -= amount;
            uiManager.UpdateUIVisuals();
            return true;
        }
        else
        {
            NewDay();
            return false;
        }
        
    }

    public void NewDay()
    {
        uiManager.FadeOut();
        Invoke("NewDayInvoke", 1.0f);
    }

    void NewDayInvoke()
    {
        //Move the player
        CharacterController c = player.GetComponent<CharacterController>();
        c.enabled = false;
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;
        c.enabled = true;
        //Update data
        currentDay++;
        RestoreStamina();
        //TEMP ANIMAL STUFF - UPDATE LATER
        for (int i = 0; i < barnManager.gameObject.transform.childCount; i++)
        {
            AnimalBehavior a = barnManager.gameObject.transform.GetChild(i).GetChild(0).GetComponent<AnimalBehavior>();
            a.readyToProduce = true;
        }
        // END TEMP ANIMAL STUFF
        uiManager.UpdateUIVisuals();
        uiManager.FadeIn();
        dayStartTime = Time.time;
        gameTime.Hour = 6;
        gameTime.Min = 0;
        //Save
        SaveFarmLayout();
        LoadFarmLayout();     
    }

    void SaveFarmLayout()
    {
        print("Saving...!");
        // TILE DATA //
        List<TileData> tiles = new List<TileData>();
        for (int r = 0; r < transform.childCount; r++)
        {
            GameObject row = transform.GetChild(r).gameObject;
            for (int i = 0; i < row.transform.childCount; i++)
            {
                GameObject t = row.transform.GetChild(i).gameObject;
                TileBehavior tileData = t.GetComponent<TileBehavior>();
                //Process new day updates - doing this here so we only need to loop all tiles once
                if (tileData.state == TileBehavior.TileState.Watered && tileData.isPlanted)
                    tileData.growthScore++;
                if (tileData.state == TileBehavior.TileState.Watered)
                    tileData.state = TileBehavior.TileState.Tilled;
                //Save the new data
                TileData tile = new TileData();
                tile.gridLoc = new Vector2 (r , i);
                tile.state = tileData.state;
                tile.cropCode = "";
                tile.soilQuality = "";
                tile.growthScore = tileData.growthScore;
                tiles.Add(tile);
                
            }
        }

        // ANIMAL DATA //
        List<AnimalData> a = new List<AnimalData>();
        //For each stall in the barn, grab that animal's data
        for (int i = 0; i < barnManager.animals.Count; i++)
        {
            //Convert to AnimalData object and add
            AnimalData new_animal = new AnimalData();
            new_animal.readyToProduce = barnManager.animals[i].readyToProduce;
            a.Add(new_animal);
        }

        // CHEST DATA //
        List<ChestData> c = new List<ChestData>();
        for (int i = 0; i < chestManager.knownChests.Count; i++)
        {
            ChestData newChest = new ChestData();
            newChest.chestInv = chestManager.knownChests[i].chestInventory;
            c.Add(newChest);
        }

        SaveData farm = new SaveData();
        farm.date = currentDay;
        farm.layout = tiles;
        farm.animals = a;
        farm.inv = playerInventory.inventoryList;
        farm.chests = c;
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    void LoadFarmLayout()
    {
        print("Loading...!");
        string json = File.ReadAllText(path);
        SaveData farm = JsonUtility.FromJson<SaveData>(json);
        // TILE DATA //
        List<TileData> tiles = farm.layout;
        for (int t = 0; t < tiles.Count; t++)
        {
            TileData tile = tiles[t];
            Vector2 tileLoc = tile.gridLoc;
            GameObject toUpdate = transform.GetChild((int)tileLoc.x).GetChild((int)tileLoc.y).gameObject;
            TileBehavior uS;
            if (uS = toUpdate.GetComponent<TileBehavior>())
            {
                uS.state = tile.state;
                uS.growthScore = tile.growthScore;
                uS.UpdateVisuals();
            }
        }
        // ANIMAL DATA //

        //For each animal in the data
        List<AnimalData> a = farm.animals;
        for (int i = 0; i < a.Count; i++)
        {
            //Get that animals stall number in the barn
            Animal animalToAlter = barnManager.animals[i];
            //Alter the animal in that stall based on data
            animalToAlter.readyToProduce = a[i].readyToProduce;
        }

        // INVENTORY DATA //
        playerInventory.inventoryList = farm.inv;
        playerInventory.UpdateInventories();

        // CHEST DATA //
        for (int i = 0; i < farm.chests.Count; i++)
        {
            //should save in the same order, therefore connected
            chestManager.knownChests[i].chestInventory = farm.chests[i].chestInv;
        }
        
    }

    [System.Serializable]
    private class TileData
    {
        public Vector2 gridLoc;
        public TileBehavior.TileState state;
        public string cropCode;
        public string soilQuality;
        public int growthScore;
    }
    [System.Serializable]
    private class AnimalData
    {
        public bool readyToProduce;
    }
    [System.Serializable]
    private class ChestData
    {
        public List<InventoryItem> chestInv;
    }

    private class SaveData
    {
        public int date;
        public List<TileData> layout;
        public List<InventoryItem> inv;
        public List<AnimalData> animals;
        public List<ChestData> chests;
    }
}
