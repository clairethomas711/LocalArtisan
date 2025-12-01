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
    [SerializeField] public InterfaceManager uiManager;
    [SerializeField] public GameObject player;
    [SerializeField] public CursorGrab grab;
    [SerializeField] public BarnManager barnManager;
    [SerializeField] public ChestManager chestManager;
    [SerializeField] public ProgressionManager progressionManager;
    [SerializeField] public GameObject staticMachines;
    [SerializeField] public GameObject notificationManager;
    [SerializeField] SunManager sunManager;
    [SerializeField] public GameplayMenu pauseMenu;
    [Header("Data Objects")]
    public ItemManifest itemManifest;
    public CraftingManifest craftingRecipeManifest;
    [Header("Game Settings")]
    [SerializeField] string path;
    [SerializeField] Transform respawnPoint;
    [SerializeField] float maxStamina;
    [SerializeField] int startingGold;
    [Header("Debug Tools")]
    [SerializeField] bool resetData;
    [SerializeField] List<string> debugStartingInventory = new List<string>();
    [SerializeField] List<int> debugStartingInventoryQuantity = new List<int>();
    [SerializeField] GameObject notification;
    //Global variables
    [HideInInspector] public int currentDay;
    [HideInInspector] public float money;
    [HideInInspector] public float stamina;

    //Manifests and large data storage
    [HideInInspector] public GameplayMenu activeMenu;
    [HideInInspector] public Inventory playerInventory;
    [HideInInspector] public Dictionary<string, ItemData> manifest = new Dictionary<string, ItemData>();
    [HideInInspector] public Dictionary<string, CraftingRecipe> recipeManifest = new Dictionary<string, CraftingRecipe>();
    [HideInInspector] public Dictionary<Vector2, Tile> tileManifest = new Dictionary<Vector2, Tile>();
    
    //Game Clock stuff
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
    [HideInInspector] public UnityEvent GameTick = new UnityEvent();
    [HideInInspector] public int totalElapsedMinutes = 0;
    float gameTimeReal;
    int priorMin = 0;
    bool newHour = false;
    bool gameRunning = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
        //BUILD THE DICTIONARYS IN THE MANIFESTS
        for (int i = 0; i < itemManifest.scriptableItems.Count; i++)
        {
            manifest[itemManifest.scriptableItems[i].id] = itemManifest.scriptableItems[i];
        }
        for (int i = 0; i < itemManifest.animalItems.Count; i++)
        {
            manifest[itemManifest.animalItems[i].id] = itemManifest.animalItems[i];
        }
        for (int i = 0; i < itemManifest.artisanItems.Count; i++)
        {
            manifest[itemManifest.artisanItems[i].id] = itemManifest.artisanItems[i];
        }
        for (int i = 0; i < itemManifest.resourceItems.Count; i++)
        {
            manifest[itemManifest.resourceItems[i].id] = itemManifest.resourceItems[i];
        }
        for (int i = 0; i < itemManifest.seedItems.Count; i++)
        {
            manifest[itemManifest.seedItems[i].id] = itemManifest.seedItems[i];
        }
        for (int i = 0; i < itemManifest.toolItems.Count; i++)
        {
            manifest[itemManifest.toolItems[i].id] = itemManifest.toolItems[i];
        }
        for (int i = 0; i < craftingRecipeManifest.scriptableItems.Count; i++)
        {
            recipeManifest[craftingRecipeManifest.scriptableItems[i].id] = craftingRecipeManifest.scriptableItems[i];
        }
    }

    void Start()
    {
        playerInventory = player.GetComponent<Inventory>();
        gameTimeReal = 0;
        gameTime.Hour = 7;

        // DEBUG SHIT //
        if (resetData || !File.Exists(path)) { GenerateNewSaveData(); }

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

    public void AddMoney(float amount)
    {
        money += amount;
        uiManager.UpdateUIVisuals();
    }

    public void SubtractMoney(float amount)
    {
        money -= amount;
        uiManager.UpdateUIVisuals();
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
        //RestoreStamina();
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
                tile.gridLoc = tileData.gridLocation;
                tile.dataPacket = tileData.GetSaveData();
                tiles.Add(tile);
                tileManifest[tile.gridLoc] = tileData;
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
            AnimalData new_animal = barnManager.animals[i];
            a.Add(new_animal);
        }

        // CHEST DATA //
        List<ChestData> c = new List<ChestData>();
        Dictionary<string, List<InventoryItem>>.KeyCollection knownChestIds = chestManager.chestManifest.Keys;
        foreach (string s in knownChestIds)
        {
            ChestData newChest = new ChestData();
            newChest.chestId = s;
            newChest.chestInv = chestManager.chestManifest[s];
            c.Add(newChest);
        }

        // PROGRESSION DATA //
        string progression = progressionManager.GetProgressionData();

        SaveData farm = new SaveData();
        farm.date = currentDay;
        farm.money = money;
        farm.layout = tiles;
        farm.animals = a;
        farm.inv = playerInventory.GetSaveData();
        farm.staticMachines = staticMachineData;
        farm.chests = c;
        farm.progression = progression;
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    void LoadFarmLayout()
    {
        SendNotification("Loading...!");
        //if (!File.Exists(path))
        //{
            //GenerateNewSaveData();
        //}
        string json = File.ReadAllText(path);
        SaveData farm = JsonUtility.FromJson<SaveData>(json);
        // BASIC DATA //
        currentDay = farm.date;
        money = farm.money;

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
                uT.gridLocation = tileLoc;
                uT.SetSaveData(tile.dataPacket);
            }
            tileManifest[tileLoc] = uT;
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
        playerInventory.SetSaveData(farm.inv);
        //playerInventory.UpdateInventories();

        // CHEST DATA //
        //Clear the current data
        chestManager.chestManifest.Clear();
        //For each chest in the save data
        for (int i = 0; i < farm.chests.Count; i++)
        {
            chestManager.chestManifest[farm.chests[i].chestId] = farm.chests[i].chestInv;
        }

        // PROGRESSION DATA //
        progressionManager.SetProgressionData(farm.progression);
        uiManager.UpdateUIVisuals();
    }

    public void GenerateNewSaveData()
    {
        SaveData farm = new SaveData();
        farm.date = 1;
        farm.money = startingGold;
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
                tileData.gridLocation = new Vector2(r, i);
                //Save the new data
                TileData tile = new TileData();
                tile.gridLoc = tileData.gridLocation;
                tile.dataPacket = tileData.GetSaveData();
                tiles.Add(tile);
                tileManifest[tile.gridLoc] = tileData;
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
                farm.inv.Add(new InventoryItem(debugStartingInventory[i], debugStartingInventoryQuantity[i]));
            }
            else
            {
                farm.inv.Add(new InventoryItem("", 0));
            }
        }
        //Animals
        farm.animals = new List<AnimalData>();
        farm.animals.Add(new AnimalData("anim_chicken", true));

        //Progression
        farm.progression = progressionManager.NewProgressionData();

        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);

        //If this is a new data file, start the game with the pause menu open
        activeMenu = pauseMenu;
        pauseMenu.Open();
        PauseGame(true);
    }

    public void DeleteData()
    {
        File.Delete(path);
        Application.Quit();
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
        public float money;
        public List<TileData> layout;
        public List<string> staticMachines;
        public List<InventoryItem> inv;
        public List<AnimalData> animals;
        public List<ChestData> chests;
        public string progression;
    }
}
