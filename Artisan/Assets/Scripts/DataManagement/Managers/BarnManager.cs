using UnityEngine;
using System.Collections.Generic;

public class BarnManager : MonoBehaviour
{
    [HideInInspector] public List<AnimalData> animals;
    [SerializeField] GameObject stalls;
    [SerializeField] GameObject cow;

    public bool AddAnimal(ItemData animalToBuy)
    {
        //Is there an empty stall?
        if (animals.Count >= stalls.transform.childCount)
            return false;
        //Add the data to the list
        print("Adding animal...");
        AnimalData newAnimal = new AnimalData();
        newAnimal.id = animalToBuy.id;
        animals.Add(newAnimal);
        //Update the barn population
        UpdateBarn();
        return true;
    }

    public void UpdateBarn()
    {
        //For each stall in the barn
        for (int i = 0; i < stalls.transform.childCount; i++)
        {
            //Clear the stall
            if (stalls.transform.GetChild(i).transform.childCount > 0)
                Destroy(stalls.transform.GetChild(i).transform.GetChild(0).gameObject);
            //Find the corresponding animal data
            if (i < animals.Count)
            {
                //Put an animal in the stall based on that data
                if (animals[i].id == "anim_cow")
                {
                    GameObject a = Instantiate(cow, stalls.transform.GetChild(i).transform);
                    a.GetComponent<AnimalBehavior>().ReadyAnimal();
                }
            }
        }
    }
}
