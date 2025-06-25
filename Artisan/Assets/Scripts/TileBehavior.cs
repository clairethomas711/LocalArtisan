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

    public void UpdateVisuals() //This should ALL be overhauled with a better seed system
    {
        if (isPlanted && transform.childCount <= 0)
            Instantiate(plantStages[0], transform.position, transform.rotation, transform);
        else if (!isPlanted && transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
        else if (isPlanted)
        {
            FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
            if (growthScore >= 3) 
            {
                Destroy(transform.GetChild(0).gameObject);
                Instantiate(plantStages[1], transform.position, transform.rotation, transform);
                state = TileState.Grown;
            }
        }

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

    public void Harvest()
    {
        FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
        fManager.AddInventoryItem(product);
        state = TileState.Tilled;
        isPlanted = false;
        UpdateVisuals();
    }
}
