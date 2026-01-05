using UnityEngine;
using System.Collections.Generic;

public abstract class Machine : Interactable
{
    [Header ("Machine Settings")]
    [SerializeField] bool initializeBroken = false;
    [SerializeField] GameObject requestChest;
    GameObject currentRequestChest;
    public GameObject indicator;
    [SerializeField] AudioClip doneSound;
    UnityEngine.UI.Slider indicatorTimer;
    UnityEngine.UI.Image indicatorDone;
    [HideInInspector] public List<InventoryItem> activelyProducing = new List<InventoryItem>();
    int quantityToProduce;
    int minutesSeen = 0;
    int minutesRequired = 0;
    public abstract List<ItemData> AcceptedItems { get; set; }
    public abstract List<ItemData> ProducedItems { get; set; }
    public MachineState state;
    public enum MachineState { ready, processing, produced, broken };

    private class MachineDataPacket
    {
        public MachineState state;
        public int minutesSeen;
        public int minutesRequired;
        public List<InventoryItem> activelyProducing;
        public int quantityToProduce;      
    }

    public override void Initialize(Tile t)
    {
        if (initializeBroken)
        {
            state = MachineState.broken;
            requestChest.SetActive(true);   
        }
        else
            state = MachineState.ready;      
    }

    public override string GetSaveData()
    {
        MachineDataPacket saveData = new MachineDataPacket();
        saveData.state = state;
        saveData.minutesSeen = minutesSeen;
        saveData.minutesRequired = minutesRequired;
        saveData.activelyProducing = new List<InventoryItem>();
        for (int i = 0; i < activelyProducing.Count; i++)
        {
            saveData.activelyProducing.Add(activelyProducing[i]);
        }
        return JsonUtility.ToJson(saveData); 
    }

    public override void SetSaveData(string saveData)
    {
        MachineDataPacket loadedData = JsonUtility.FromJson<MachineDataPacket>(saveData);
        state = loadedData.state;
        if (state == MachineState.broken)
        {
            requestChest.SetActive(true);
        }
        minutesSeen = loadedData.minutesSeen;
        minutesRequired = loadedData.minutesRequired;
        activelyProducing.Clear();
        for (int i = 0; i < loadedData.activelyProducing.Count; i++)
        {
            activelyProducing.Add(loadedData.activelyProducing[i]);
        }
        if (state == MachineState.processing)
        {
            indicator.SetActive(true);
            DataManager.instance.GameTick.AddListener(MachineTickListener); //Start listening to GameTick again       
        }
    }
    
    public override void NewDay()
    {
        if (state == MachineState.processing)
            minutesSeen += 200;
    }

    //Moved this from abstract to be universal
    public void MachineTickListener()
    {
        if (state == MachineState.processing)
        {
            minutesSeen++;
            indicatorTimer = indicator.transform.GetChild(0).GetComponent<UnityEngine.UI.Slider>();
            indicatorTimer.value = (float)minutesSeen / minutesRequired;
            if (minutesSeen >= minutesRequired)
            {
                Produced();
            }
        }
    }

    // UNIVERSAL HELPER FUNCTIONS //
    public void StartProducing(InventoryItem output, InventoryItem secondaryOutput = null)
    {
        //print("Producing " + output.customItemData);
        activelyProducing.Add(output); //Remember what the output will be
        if (secondaryOutput != null) activelyProducing.Add(secondaryOutput);
        //minOfProductionStart = DataManager.instance.TotalElapsedGameTime(); //Save when we started producing
        minutesRequired = CalculateProcessingTime();
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
        minutesSeen = 0;
        minutesRequired = 0;
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
        DataManager.instance.playerInventory.AddInventoryItem(activelyProducing[0]); //Add the item(s) to the player's inventory
        if (activelyProducing.Count > 1) DataManager.instance.playerInventory.AddInventoryItem(activelyProducing[1]);
        state = MachineState.ready; //We are now ready for new input
        activelyProducing.Clear(); //Reset what we are producing
        //Visual feedback (change later)
        indicator.SetActive(false);
        indicatorTimer.gameObject.SetActive(true);
        indicatorDone.gameObject.SetActive(false);
        OnProductCollection();
    }

    public abstract int CalculateProcessingTime();

    public abstract void OnProductCollection();

}
