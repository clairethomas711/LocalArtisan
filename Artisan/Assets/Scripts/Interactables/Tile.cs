using UnityEngine;
using System.Collections.Generic;

public class Tile : Interactable
{
    public bool isStatic = false;
    [Header("Visuals")]
    [SerializeField] private Texture grass;
    [SerializeField] private Texture tilled;
    [SerializeField] private Texture watered;
    [SerializeField] GameObject trash;

    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Grown, Trashed, Static };
    [HideInInspector] public TileState state;
    [HideInInspector] public int growthScore;
    private string product;
    public bool isPlanted = false;
    List<GameObject> plantStages;
    /*void Start()
    {
        if (isStatic)
            state = TileState.Static;
        else if (SpawnTrash(0.3f) == false)
            state = TileState.Untilled;
    }*/

    public override string Interact(InventoryItem currentItem)
    {
        if (state == TileState.Grown)
        {
            UseHarvest();
            return "";
        }
        if (currentItem.id == "") { return ""; }
        switch (DataManager.instance.manifest[currentItem.id].itemType) //Need one case for each item enum type
        {
            case itemType.Hoe:
                return UseHoe();

            case itemType.WateringCan:
                return UseWateringCan();

            case itemType.Seed:
                UseSeed((Seed)DataManager.instance.manifest[currentItem.id]);
                return "";
        }
        return "";
    }

    private string UseHoe()
    {
        if (Till())
        {
            DataManager.instance.SubtractStamina(2);
            return "Hit";
        }
        return "";
    }

    private string UseWateringCan()
    {
        if (Water())
        {
            DataManager.instance.SubtractStamina(2);
            return "Water";
        }
        return "";
    }

    private void UseSeed(Seed currentItem)
    {
        if (Plant(currentItem))
            DataManager.instance.SubtractStamina(1);
    }

    private void UseHarvest()
    {
        if (Harvest())
            DataManager.instance.SubtractStamina(1);
    }
    
    public void UpdateVisuals()
    {
        if (gameObject.layer != 3 && gameObject.layer != 6) return;
        GrowPlant();

        MeshRenderer mat = GetComponent<MeshRenderer>();
        if (state == TileState.Untilled)
        {
            mat.material.mainTexture = grass;
        }
        else if (state == TileState.Tilled)
        {
            
            mat.material.mainTexture = tilled;
        }
        else if (state == TileState.Watered)
        {
            mat.material.mainTexture = watered;
        }
    }

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
        if ((state == TileState.Tilled || state == TileState.Watered) && !isPlanted)
        {
            isPlanted = true;
            plantStages = s.stages;
            product = s.product.id;
            growthScore = 0;
            DataManager.instance.playerInventory.RemoveInventoryItem(s.id);
            UpdateVisuals();
            return true;
        }
        return false;
    }

    void GrowPlant()
    {
        if (transform.childCount > 0) //Clear existing plant
                Destroy(transform.GetChild(0).gameObject);
        if (state == TileState.Trashed) //If this tile is trashed, keep it trashed
        {
            Instantiate(trash, transform);
            return;
        }
        if (!isPlanted) //If we harvested, don't grow again
                return;
        if (growthScore >= plantStages.Count - 1)
            state = TileState.Grown;

        Quaternion properRotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
        Instantiate(plantStages[growthScore], transform.position, properRotation, transform); //Spawn new plant
    }

    public bool Harvest()
    {
        DataManager.instance.playerInventory.AddInventoryItem(product);
        state = TileState.Tilled;
        isPlanted = false;
        UpdateVisuals();
        return true;
    }

    public bool SpawnTrash(float chance)
    {
        if (chance >= Random.Range(0.0f, 1.0f))
        {
            state = TileState.Trashed;
            Instantiate(trash, transform);
            return true;
        }
        return false;
    }
}
