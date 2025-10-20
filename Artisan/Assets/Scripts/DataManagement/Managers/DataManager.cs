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
    [SerializeField] public GameObject staticMachines;
    [SerializeField] public GameObject notificationManager;
    [SerializeField] SunManager sunManager;
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
    [SerializeField] GameObject notification;
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
            if (gameTime.Hour == 19)
            {
                sunManager.isSunSet = true;
            }
            else if (gameTime.Hour == 24)
            {
                SendNotification("It's too late at night -  you passed out!");
                NewDay();
            }
        }
        else if (gameTime.Min > 0)
        {
            newHour = true;
        }
        //Visuals Update
        uiManager.UpdateClock();
        sunManager.UpdateSun();
    }

    // UNIVERSAL HELPER FUNCTIONS //
    public void SendNotification(string n)
    {
        GameObject notif = Instantiate(notification, notificationManager.transform);
        notif.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = n;
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
            if (stamina <= 10f)
            {
                SendNotification("Be careful! You're running out of energy.");
            }
            return true;
        }
        else
        {
            SendNotification("You passed out!");
            NewDay();
            return false;
        }
        
    }

    public void NewDay()
    {
        uiManager.FadeOut();
        Invoke("NewDayInvoke", 1.0f);
    }

    // SAVE GAME STUFF //
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
        sunManager.ResetSun();
        LoadFarmLayout();
        gameTimeReal = 0;     
    }

    void SaveFarmLayout()
    {
        SendNotification("Saving...!");
        // TILE DATA //
        List<TileData> tiles = new List<TileData>();
        //Loop over all of the children of the DataManager object (should only be Tile objects)
        for (int r = 0; r < transform.childCount; r++)
        {
            GameObject row = transform.GetChild(r).gameObject;
            for (int i = 0; i < row.transform.childCount; i++)
            {
                Tile tileData = row.transform.GetChild(i).gameObject.GetComponent<Tile>();
                //Process new day updates - doing this here so we only need to loop all tiles once. SHOULD PROBABLY BE A FUNCTION???
                tileData.NewDay();
                //Save the new data
                TileData tile = new TileData();
                tile.gridLoc = new Vector2(r, i);
                tile.dataPacket = tileData.GetSaveData();
                tiles.Add(tile);
            }
        }
        // STATIC MACHINES //
        List<string> staticMachineData = new List<string>();
        for (int i = 0; i < staticMachines.transform.childCount; i++)
        {
            Interactable machine = staticMachines.transform.GetChild(i).GetComponent<Interactable>();
            machine.NewDay();
            staticMachineData.Add(machine.GetSaveData());
        }

        // ANIMAL DATA - THIS NEEDS TO BE UPDATED WITH THE NEW SYSTEM //
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
        Dictionary<string, List<InventoryItem>>.KeyCollection knownChestIds = chestManager.chestManifest.Keys;
        foreach (string s in knownChestIds )
        {
            ChestData newChest = new ChestData();
            newChest.chestId = s;
            newChest.chestInv = chestManager.chestManifest[s];
            c.Add(newChest);
        }

        SaveData farm = new SaveData();
        farm.date = currentDay;
        farm.layout = tiles;
        farm.animals = a;
        farm.inv = playerInventory.inventoryList;
        farm.staticMachines = staticMachineData;
        farm.chests = c;
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    void LoadFarmLayout()
    {
        SendNotification("Loading...!");
        if (!File.Exists(path))
        {
            GenerateNewSaveData();
        }
        string json = File.ReadAllText(path);
        SaveData farm = JsonUtility.FromJson<SaveData>(json);
        // TILE DATA //
        List<TileData> tiles = farm.layout;
        //Loop over the saved tile data
        for (int t = 0; t < tiles.Count; t++)
        {
            TileData tile = tiles[t];
            Vector2 tileLoc = tile.gridLoc;
            //Grab the gameobject that can be found at the saved grid location for that tile
            GameObject toUpdate = transform.GetChild((int)tileLoc.x).GetChild((int)tileLoc.y).gameObject;
            //Update the data for that tile based on the save data
            Tile uT;
            if (uT = toUpdate.GetComponent<Tile>())
            {
                uT.SetSaveData(tile.dataPacket);
            }
        }

        // STATIC MACHINES //
        for (int i = 0; i < farm.staticMachines.Count; i++)
        {
            staticMachines.transform.GetChild(i).gameObject.GetComponent<Interactable>().SetSaveData(farm.staticMachines[i]);       
        }

        // ANIMAL DATA - NEEDS TO BE UPDATED WITH THE NEW SYSTEM //
        barnManager.animals = farm.animals;
        barnManager.UpdateBarn();

        // INVENTORY DATA //
        playerInventory.inventoryList = farm.inv;
        playerInventory.UpdateInventories();

        // CHEST DATA //
        //Clear the current data
        chestManager.chestManifest.Clear();
        //For each chest in the save data
        for (int i = 0; i < farm.chests.Count; i++)
        {
            chestManager.chestManifest[farm.chests[i].chestId] = farm.chests[i].chestInv;
        }
        
    }

    void GenerateNewSaveData()
    {
        SaveData farm = new SaveData();
        farm.date = 0;
        // TILES (INCLUDES RANDOMIZATION)
        List<TileData> tiles = new List<TileData>();
        for (int r = 0; r < transform.childCount; r++)
        {
            GameObject row = transform.GetChild(r).gameObject;
            for (int i = 0; i < row.transform.childCount; i++)
            {
                GameObject t = row.transform.GetChild(i).gameObject;
                Tile tileData = t.GetComponent<Tile>();
                //THIS NEEDS TO BE CHANGED - generating stuff
                tileData.GenerateNewData();
                tileData.UpdateVisuals();
                //Save the new data
                TileData tile = new TileData();
                tile.gridLoc = new Vector2(r, i);
                tile.dataPacket = tileData.GetSaveData();
                tiles.Add(tile);
            }
        }
        farm.layout = tiles;
        //Static machines (includes static chests)
        farm.staticMachines = new List<string>();
        for (int i = 0; i < staticMachines.transform.childCount; i++)
        {
            Interactable machine = staticMachines.transform.GetChild(i).GetComponent<Interactable>();
            machine.Initialize(null);
            farm.staticMachines.Add(machine.GetSaveData());
        }
        farm.chests = new List<ChestData>();
        Dictionary<string, List<InventoryItem>>.KeyCollection knownChestIds = chestManager.chestManifest.Keys;
        foreach (string s in knownChestIds )
        {
            ChestData newChest = new ChestData();
            newChest.chestId = s;
            newChest.chestInv = chestManager.chestManifest[s];
            farm.chests.Add(newChest);
        }
        //Inventory
        farm.inv = new List<InventoryItem>();
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
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    [System.Serializable]
    private class TileData
    {
        public Vector2 gridLoc;
        public string dataPacket;
    }
    
    [System.Serializable]
    private class ChestData
    {
        public string chestId;
        public List<InventoryItem> chestInv;
    }

    private class SaveData
    {
        public int date;
        public List<TileData> layout;
        public List<string> staticMachines;
        public List<InventoryItem> inv;
        public List<AnimalData> animals;
        public List<ChestData> chests;
    }
}
