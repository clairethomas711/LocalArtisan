using UnityEngine;

//Attached to the Tile prefab object, stores the tile's current state and allows items to be used on it.

public class TileBehavior : MonoBehaviour
{
    [SerializeField] private Texture grass;
    [SerializeField] private Texture tilled;
    [SerializeField] private Texture watered;

    public enum TileState { Untilled, Tilled, Watered, Grown };
    public TileState state;
    public GameObject seed;
    public GameObject plant;
    private InventorySlot product;
    public int plantedDate;

    void Start()
    {
        state = TileState.Untilled;
    }

    public void UpdateVisuals() //This should ALL be overhauled with a better seed system
    {
        if (seed != null && transform.childCount <= 0)
            Instantiate(seed, transform.position, transform.rotation, transform);
        else if (seed == null && transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
        else if (seed != null)
        {
            FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
            if (fManager.currentDay > plantedDate)
            {
                Destroy(transform.GetChild(0).gameObject);
                Instantiate(plant, transform.position, transform.rotation, transform);
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

    public void Plant(GameObject mesh1, GameObject mesh2, InventorySlot p)
    {
        if ((state == TileState.Tilled || state == TileState.Watered) && seed == null)
        {
            seed = mesh1;
            plant = mesh2;
            product = p;
            FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
            plantedDate = fManager.currentDay;
            UpdateVisuals();
        }
    }

    public void Harvest()
    {
        FarmManager fManager = transform.parent.gameObject.GetComponent<FarmManager>();
        fManager.AddInventoryItem(product);
        state = TileState.Untilled;
        seed = null;
        UpdateVisuals();
    }
}
