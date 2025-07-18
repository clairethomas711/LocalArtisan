using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    public int processingTimeInMinutes;
    public GameObject indicator;
    [HideInInspector] public ItemData activelyProducing;
    [HideInInspector] public int minOfProductionStart;
    [HideInInspector] public int minOfProductionEnd;
    public abstract List<ItemData> AcceptedItems { get; set; }
    public abstract List<ItemData> ProducedItems { get; set; }
    [HideInInspector] public MachineState state;
    public enum MachineState { ready, processing, produced };

    void Start()
    {
        
        state = MachineState.ready;
    }

    public abstract void MachineTickListener();

    // UNIVERSAL HELPER FUNCTIONS //
    public void StartProducing(ItemData output)
    {
        DataManager.instance.GameTick.AddListener(MachineTickListener); //Start listening to GameTick
        state = MachineState.processing; //We are now processing
        activelyProducing = output; //Remember what the output will be
        minOfProductionStart = DataManager.instance.GameTimeInMinutes(); //Save when we started producing
        minOfProductionEnd = minOfProductionStart + processingTimeInMinutes; //Calculate when we will end producing
        //Visual feedback (change later)
        indicator.GetComponent<MeshRenderer>().material.color = Color.red; 
        indicator.SetActive(true);
    }

    public void Produced()
    {
        state = MachineState.produced; //We have finished processing
        DataManager.instance.GameTick.RemoveListener(MachineTickListener); //Stop listening to GameTick
        //Visual feedback (change later)
        indicator.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public void TakeProducedItem()
    {
        DataManager.instance.playerInventory.AddInventoryItem(activelyProducing.id); //Add the item to the player's inventory
        state = MachineState.ready; //We are now ready for new input
        activelyProducing = null; //Reset what we are producing
        //Visual feedback (change later)
        indicator.GetComponent<MeshRenderer>().material.color = Color.white;
        indicator.SetActive(false);
    }

}
