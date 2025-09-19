using UnityEngine;
using System.Collections.Generic;

//Attached to the Tile prefab object, stores the tile's current state and allows items to be used on it.

public class TileBehavior : MonoBehaviour
{
    [SerializeField] private Texture grass;
    [SerializeField] private Texture tilled;
    [SerializeField] private Texture watered;
    [SerializeField] GameObject trash;

    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Grown, Trashed };
    [HideInInspector] public TileState state;
    [HideInInspector] public int growthScore;
    private string product;
    public bool isPlanted = false;
    List<GameObject> plantStages;

    void Start()
    {
        if (SpawnTrash(0.3f) == false)
            state = TileState.Untilled;
    }

    public void UpdateVisuals()
    {
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
        if (state == TileState.Trashed)
            return;
        if (transform.childCount > 0) //Clear existing plant
                Destroy(transform.GetChild(0).gameObject);
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
        if (chance <= Random.Range(0.0f, 1.0f))
        {
            state = TileState.Trashed;
            Instantiate(trash, transform);
            return true;
        }
        return false;
    }
}
