using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects / Quest Data")]
public class QuestData : ScriptableObject
{
    public string questTitle;
    public string id;
    public List<QuestTask> questTasks;
    public List<QuestReward> questRewards;
}

[System.Serializable]
public class QuestTask
{
    public taskType taskType;
    public int taskQuantity;
    public string taskItemId;
    public string description;
}

[System.Serializable]
public class QuestReward
{
    public rewardType rewardType;
    public int rewardQuantity;
    public string rewardId;
}

public enum taskType { CollectItem, PlaceItem, WaterCrop, HarvestCrop, StockGood, InteractItem }
public enum rewardType { Money, Quest, Flag }
