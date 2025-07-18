using UnityEngine;
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
    [SerializeField] GameObject animalList;
    [Header("Data Objects")]
    public ItemManifest itemManifest;
    [Header("Game Settings")]
    [SerializeField] string path;
    [SerializeField] Transform respawnPoint;
    [SerializeField] float maxStamina;

    [HideInInspector] public int currentDay;
    [HideInInspector] public int money;
    [HideInInspector] public float stamina;

    [HideInInspector] public Inventory playerInventory;
    [HideInInspector] public Dictionary<string, ItemData> manifest = new Dictionary<string, ItemData>();
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
    float dayStartTime;
    float gameTimeReal;
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
    }

    void Update() 
    {
        GameClockProgress();
    }

    void GameClockProgress() //Game Clock
    {
        gameTimeReal = Time.time;
        float dif = gameTimeReal - dayStartTime;
        gameTime.Min = (int)Mathf.Floor(dif % 60);
        if (gameTime.Min == 0 && newHour)
        {
            gameTime.Hour += 1;
            newHour = false;
        }
        else if (gameTime.Min > 0)
        {
            newHour = true;
        }
        uiManager.UpdateClock();
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
        for (int i = 0; i < animalList.transform.childCount; i++)
        {
            Animal a = animalList.transform.GetChild(i).GetComponent<Animal>();
            a.readyToProduce = true;
        }
        store.SellAllItems();
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
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject t = transform.GetChild(i).gameObject;
            TileBehavior tileData = t.GetComponent<TileBehavior>();
            //Process new day updates
            if (tileData.state == TileBehavior.TileState.Watered && tileData.isPlanted)
                tileData.growthScore++;
            if (tileData.state == TileBehavior.TileState.Watered)
                tileData.state = TileBehavior.TileState.Tilled;
            //Save the new data
                Tile tile = new Tile();
            tile.gridLoc = t.transform.position;
            tile.state = tileData.state;
            tile.cropCode = "";
            tile.soilQuality = "";
            tile.growthScore = tileData.growthScore;
            tiles.Add(tile);
        }
        FarmLayout farm = new FarmLayout();
        farm.date = currentDay;
        farm.layout = tiles;
        string json = JsonUtility.ToJson(farm);
        File.WriteAllText(path, json);
    }

    void LoadFarmLayout()
    {
        print("Loading...!");
        string json = File.ReadAllText(path);
        FarmLayout farm = JsonUtility.FromJson<FarmLayout>(json);
        List<Tile> tiles = farm.layout;
        for (int i = 0; i < tiles.Count; i++)
        {
            Tile tile = tiles[i];
            GameObject toUpdate = transform.GetChild(i).gameObject;
            toUpdate.transform.position = tile.gridLoc;
            TileBehavior uS = toUpdate.GetComponent<TileBehavior>();
            uS.state = tile.state;
            uS.growthScore = tile.growthScore;
            uS.UpdateVisuals();
        }
    }

    [System.Serializable]
    private class Tile
    {
        public Vector3 gridLoc;
        public TileBehavior.TileState state;
        public string cropCode;
        public string soilQuality;
        public int growthScore;
    }

    private class FarmLayout
    {
        public int date;
        public List<Tile> layout;
    }
}
