using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    public int processingTimeInMinutes;
    public GameObject indicator;
    [SerializeField] AudioClip doneSound;
    UnityEngine.UI.Slider indicatorTimer;
    UnityEngine.UI.Image indicatorDone;
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

    //Moved this from abstract to be universal
    public void MachineTickListener()
    {
        indicatorTimer.value = ((float)DataManager.instance.TotalElapsedGameTime() - minOfProductionStart) / (minOfProductionEnd - minOfProductionStart);
        if (state == MachineState.processing)
        {
            if (DataManager.instance.TotalElapsedGameTime() >= minOfProductionEnd)
            {
                Produced();
            }
        }
    }

    // UNIVERSAL HELPER FUNCTIONS //
    public void StartProducing(ItemData output)
    {
        activelyProducing = output; //Remember what the output will be
        minOfProductionStart = DataManager.instance.TotalElapsedGameTime(); //Save when we started producing
        minOfProductionEnd = CalculateProcessingTime(minOfProductionStart);
        state = MachineState.processing; //We are now processing
        DataManager.instance.GameTick.AddListener(MachineTickListener); //Start listening to GameTick
        //Visual feedback 
        indicatorTimer = indicator.transform.GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        indicator.SetActive(true);
    }

    public void Produced()
    {
        state = MachineState.produced; //We have finished processing
        DataManager.instance.GameTick.RemoveListener(MachineTickListener); //Stop listening to GameTick
        //Visual feedback
        if (doneSound)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(doneSound);
        }
        indicatorDone = indicator.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        indicatorTimer.gameObject.SetActive(false);
        indicatorDone.gameObject.SetActive(true);
    }

    public void TakeProducedItem()
    {
        DataManager.instance.playerInventory.AddInventoryItem(activelyProducing.id); //Add the item to the player's inventory
        state = MachineState.ready; //We are now ready for new input
        activelyProducing = null; //Reset what we are producing
        //Visual feedback (change later)
        indicator.SetActive(false);
        indicatorTimer.gameObject.SetActive(true);
        indicatorDone.gameObject.SetActive(false);
    }

    public abstract int CalculateProcessingTime(int minOfProductionStart);

}
