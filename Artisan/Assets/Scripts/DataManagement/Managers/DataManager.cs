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
    [SerializeField] int startingGold;
    [Header("Debug Tools")]
    [SerializeField] bool resetData;
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
    float gameTimeReal;
    int priorMin = 0;
    bool newHour = false;
    bool gameRunning = true;

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
        gameTimeReal = 0;
        gameTime.Hour = 6;
        AddMoney(startingGold);

        // DEBUG SHIT //
        if (resetData) { GenerateNewSaveData(); }

        LoadFarmLayout();
    }

    void Update() 
    {
        if (gameRunning)
            GameClockProgress();
    }

    void GameClockProgress() //Game Clock
    {
        //Convert the real time to game time
        gameTimeReal += Time.deltaTime;
        gameTime.Min = (int)Mathf.Floor(gameTimeReal % 60);
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

    public void PauseGame(bool pause)
    {
        if (pause)
            gameRunning = false;
        else
            gameRunning = true;
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
        // ANIMALS //
        for (int i = 0; i < barnManager.animals.Count; i++)
        {
            AnimalData a = barnManager.animals[i];
            a.readyToProduce = true;
        }
        uiManager.UpdateUIVisuals();
        uiManager.FadeIn();
        gameTime.Hour = 6;
        gameTime.Min = 0;
        //Save
        SaveFarmLayout();
        LoadFarmLayout();
        gameTimeReal = 0;     
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
                Tile tileData = t.GetComponent<Tile>();
                //Process new day updates - doing this here so we only need to loop all tiles once
                if (tileData.state == Tile.TileState.Watered && tileData.isPlanted)
                    tileData.growthScore++;
                if (tileData.state == Tile.TileState.Watered)
                    tileData.state = Tile.TileState.Tilled;
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
            new_animal = barnManager.animals[i];
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
        if (!File.Exists(path))
        {
            GenerateNewSaveData();
        }
        string json = File.ReadAllText(path);
        SaveData farm = JsonUtility.FromJson<SaveData>(json);
        // TILE DATA //
        List<TileData> tiles = farm.layout;
        for (int t = 0; t < tiles.Count; t++)
        {
            TileData tile = tiles[t];
            Vector2 tileLoc = tile.gridLoc;
            GameObject toUpdate = transform.GetChild((int)tileLoc.x).GetChild((int)tileLoc.y).gameObject;
            Tile uS;
            if (uS = toUpdate.GetComponent<Tile>())
            {
                uS.state = tile.state;
                uS.growthScore = tile.growthScore;
                uS.UpdateVisuals();
            }
        }
        // ANIMAL DATA //
        barnManager.animals = farm.animals;
        barnManager.UpdateBarn();

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

    void GenerateNewSaveData()
    {
        SaveData farm = new SaveData();
        farm.date = 0;
        //Clear all tiles, create random trash
        List<TileData> tiles = new List<TileData>();
        for (int r = 0; r < transform.childCount; r++)
        {
            GameObject row = transform.GetChild(r).gameObject;
            for (int i = 0; i < row.transform.childCount; i++)
            {
                GameObject t = row.transform.GetChild(i).gameObject;
                Tile tileData = t.GetComponent<Tile>();
                if (tileData.isStatic)
                    tileData.state = Tile.TileState.Static;
                else if (tileData.SpawnTrash(0.3f) == false)
                    tileData.state = Tile.TileState.Untilled;
                //Save the new data
                TileData tile = new TileData();
                tile.gridLoc = new Vector2(r, i);
                tile.state = tileData.state;
                tile.cropCode = "";
                tile.soilQuality = "";
                tile.growthScore = tileData.growthScore;
                tiles.Add(tile);
            }
        }
        farm.layout = tiles;
        farm.inv = new List<InventoryItem>();
        //Inventory
        for (int i = 0; i < playerInventory.maxCapacity; i++)
        {
            if (i < debugStartingInventory.Count)
            {
                farm.inv.Add(new InventoryItem(debugStartingInventory[i], 1));
            }
            else
            {
                farm.inv.Add(new InventoryItem("", 0));
            }
        }
        //Animals
        farm.animals = null;
        //Chests
        farm.chests = new List<ChestData>();
        for (int i = 0; i < chestManager.transform.childCount; i++)
        {
            ChestData newChest = new ChestData();
            newChest.chestInv = new List<InventoryItem>();
            for (int j = 0; j < chestManager.knownChests[i].chestCapacity; j++)
            {
                newChest.chestInv.Add(new InventoryItem("", 0));
            }
            farm.chests.Add(newChest);
        }
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    [System.Serializable]
    private class TileData
    {
        public Vector2 gridLoc;
        public Tile.TileState state;
        public string cropCode;
        public string soilQuality;
        public int growthScore;
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
