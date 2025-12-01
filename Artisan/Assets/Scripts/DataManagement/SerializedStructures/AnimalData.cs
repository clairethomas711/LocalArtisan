using UnityEngine;

[System.Serializable]
public class AnimalData
{
    public string id;
    public bool readyToProduce;

    public AnimalData(string i, bool ready) 
    {
        id = i;
        readyToProduce = ready;
    }
}
