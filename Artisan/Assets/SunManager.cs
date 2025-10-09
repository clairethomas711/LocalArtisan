using UnityEngine;

public class SunManager : MonoBehaviour
{
    [SerializeField] Transform sunCenter;
    [HideInInspector] public bool isSunSet = false;
    Light sun;
    Vector3 startingPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.LookAt(sunCenter);
        sun = GetComponent<Light>();
        startingPosition = transform.position;
    }

    // Update is called once per frame
    public void UpdateSun()
    {
        if (isSunSet)
        {
            sun.intensity -= 0.001f;
            if (sun.intensity <= 0.1f)
            {
                isSunSet = false;
            }
        }
        //1sec/min -> 20hr in game = 20min in game -> 1200sec -> 180 degrees -> 180/1200 = 0.15
        transform.RotateAround(sunCenter.position, -Vector3.forward, 0.15f * Time.deltaTime);
    }

    public void ResetSun()
    {
        transform.position = startingPosition;
        transform.LookAt(sunCenter);
        isSunSet = false;
    }
}
