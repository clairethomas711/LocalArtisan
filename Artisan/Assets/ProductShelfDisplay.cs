using UnityEngine;

public class ProductShelfDisplay : MonoBehaviour
{
    public void UpdateProductShelfDisplay(int count) //This should be called IMMEDIATELY when we instantiate a product
    {
        if (count > transform.childCount)
        {
            count = transform.childCount;
        }
        for (int i = 0; i < count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);       
        }
    }
}
