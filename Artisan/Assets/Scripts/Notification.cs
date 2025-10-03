using UnityEngine;

public class Notification : MonoBehaviour
{
    float startTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= 5.0f)
        {
            Destroy(gameObject);
        }
    }
}
