using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

//A manager EXCLUSIVELY for the farm scene

public class FarmManager : MonoBehaviour
{

    [SerializeField] private GameObject tileObject;
    [SerializeField] string path;
    [SerializeField] Inventory playerInventory;
    [SerializeField] TextMeshProUGUI moneyText;

    [HideInInspector] public int currentDay;
    [HideInInspector] public int money;

    void Start() { currentDay = 1; }

    public void AddInventoryItem(InventorySlot i)
    {
        for (int j = 0; j < playerInventory.inventoryList.Count; j++)
        {
            if (playerInventory.inventoryList[j].name == i.name) //Increase quantity
            {
                playerInventory.inventoryList[j].quantity++;
                playerInventory.DisplayInventory();
                return;
            }
        }
        //If nothing with the same name found, add
        playerInventory.inventoryList.Add(i);
        playerInventory.DisplayInventory();
    }

    public void RemoveInventoryItem(InventorySlot i)
    {
        for (int j = 0; j < playerInventory.inventoryList.Count; j++)
        {
            if (playerInventory.inventoryList[j].name == i.name)
            {
                if (playerInventory.inventoryList[j].quantity > 1) //Decrease quantity
                {
                    playerInventory.inventoryList[j].quantity--;
                    playerInventory.DisplayInventory();
                    return;
                }
                else
                {
                    playerInventory.inventoryList.Remove(playerInventory.inventoryList[j]);
                    playerInventory.DisplayInventory();
                }
            }
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        moneyText.text = money.ToString();
    }

    public void NewDay()
    {
        currentDay++;
        SaveFarmLayout();
        LoadFarmLayout();
        print("Day: " + currentDay);
    }

    void SaveFarmLayout()
    {
        print("Saving...!");
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject t = transform.GetChild(i).gameObject;
            Tile tile = new Tile();
            tile.gridLoc = t.transform.position;
            tile.state = t.GetComponent<TileBehavior>().state;
            tile.cropCode = "";
            tile.soilQuality = "";
            tile.plantedDate = t.GetComponent<TileBehavior>().plantedDate;
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
        currentDay = farm.date;
        for (int i = 0; i < tiles.Count; i++)
        {
            Tile tile = tiles[i];
            GameObject toUpdate = transform.GetChild(i).gameObject;
            toUpdate.transform.position = tile.gridLoc;
            TileBehavior uS = toUpdate.GetComponent<TileBehavior>();
            uS.state = tile.state;
            uS.plantedDate = tile.plantedDate;
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
        public int plantedDate;
    }

    private class FarmLayout
    {
        public int date;
        public List<Tile> layout;
    }
}
