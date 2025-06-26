using UnityEngine;
using System.Collections.Generic;

//Attached to the Tile prefab object, stores the tile's current state and allows items to be used on it.

public class TileBehavior : MonoBehaviour
{
    [SerializeField] private Texture grass;
    [SerializeField] private Texture tilled;
    [SerializeField] private Texture watered;

    [HideInInspector] public enum TileState { Untilled, Tilled, Watered, Grown };
    [HideInInspector] public TileState state;
    [HideInInspector] public int growthScore;
    private InventorySlot product;
    public bool isPlanted = false;
    List<GameObject> plantStages;

    void Start()
    {
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

    public void Till()
    {

        if (state == TileState.Untilled || state == TileState.Watered)
        {
            state = TileState.Tilled;
        }
        else if (state == TileState.Tilled)
        {
            state = TileState.Untilled;
        }

        UpdateVisuals();
    }

    public void Water()
    {
        if (state == TileState.Tilled)
        {
            state = TileState.Watered;
        }

        UpdateVisuals();
    }

    public void Plant(Seed s)
    {
        if ((state == TileState.Tilled || state == TileState.Watered) && !isPlanted)
        {
            isPlanted = true;
            plantStages = s.stages;
            product = s.product;
            FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
            growthScore = 0;
            fManager.RemoveInventoryItem(s);
            UpdateVisuals();
        }
    }

    void GrowPlant()
    {
        if (transform.childCount > 0) //Clear existing plant
            Destroy(transform.GetChild(0).gameObject); 
        if (!isPlanted) //If we harvested, don't grow again
            return;
        if (growthScore >= plantStages.Count - 1)
            state = TileState.Grown;

        Quaternion properRotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z));
        Instantiate(plantStages[growthScore], transform.position, properRotation, transform); //Spawn new plant
    }

    public void Harvest()
    {
        FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
        fManager.AddInventoryItem(product);
        state = TileState.Tilled;
        isPlanted = false;
        UpdateVisuals();
    }
}
