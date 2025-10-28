using UnityEngine;

public class ProductShelfDisplay : MonoBehaviour
{
    [SerializeField] int materialInstanceToAlter = 0;
    public void UpdateProductShelfDisplay(int count, Vector3? customColor = null) //This should be called IMMEDIATELY when we instantiate a product
    {
        if (count > transform.childCount)
        {
            count = transform.childCount;
        }
        for (int i = 0; i < count; i++)
        {
            GameObject target = transform.GetChild(i).gameObject;
            target.SetActive(true);
            if (customColor != null)
            {
                Vector3 c = (Vector3)customColor;
                target.GetComponent<MeshRenderer>().materials[materialInstanceToAlter].color = new Color(c.x, c.y, c.z, 1.0f);
            }       
        }
    }
}
