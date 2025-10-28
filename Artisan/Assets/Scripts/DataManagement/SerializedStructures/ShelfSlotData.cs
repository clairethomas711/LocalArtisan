using UnityEngine;

public class ShelfSlotData : InventorySlotData
{
    [SerializeField] Transform slotVisualSpawnPoint;

    public void UpdateShelfDisplay()
    {
        //If there is a spawned prefab, remove it to prepare for update
        if (slotVisualSpawnPoint.childCount > 0)
            Destroy(slotVisualSpawnPoint.GetChild(0).gameObject);
        if (currentItem.id != "") //As long as this isn't a blank item, we will put a product display there
        {
            Artisan itemData = (Artisan)DataManager.instance.manifest[currentItem.id];
            GameObject toSpawn = itemData.model;
            //Spawn the items model
            GameObject spawned = Instantiate(toSpawn, slotVisualSpawnPoint.position, slotVisualSpawnPoint.rotation, slotVisualSpawnPoint);
            //Once the product model is spawned, let it know how many we should display
            CustomInventoryItemData data = currentItem.GetCustomData();
            if (data != null)
            {
                spawned.GetComponent<ProductShelfDisplay>().UpdateProductShelfDisplay(currentItem.quantity, data.customColor);
            }
            else
                spawned.GetComponent<ProductShelfDisplay>().UpdateProductShelfDisplay(currentItem.quantity);
        }
    }
}
