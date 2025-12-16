using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects / Quest Data")]
public class QuestData : ScriptableObject
{
    public string questTitle;
    public string id;
    public List<QuestTask> questTasks;

}

[System.Serializable]
public class QuestTask
{
    public taskType taskType;
    public int taskQuantity;
    public string taskItemId;
    public string description;
}

public enum taskType { CollectItem }
