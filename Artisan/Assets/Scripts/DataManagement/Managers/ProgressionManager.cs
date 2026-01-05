using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionManager : MonoBehaviour
{
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject questHintPrefab;
    [SerializeField] List<string> knownFlags; //all start false
    public Dictionary<string, int> knownSpecializations = new Dictionary<string, int>(); //All the specializations and how many hours we have in each
    public Dictionary<string, int> knownRecipes = new Dictionary<string, int>(); //All recipes we've made and how many times we've made it
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>(); //List of active quests
    [HideInInspector] public List<string> completedQuests = new List<string>(); //List of previously completed quests
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    //Large Progression Data Struct
    [System.Serializable]
    private class ProgressionData
    {
        public List<SpecializationProgressionData> knownSpecializations;
        public List<RecipeProgressionData> knownRecipes;
        public List<QuestProgressionData> activeQuests;
        public List<string> completedQuestIds;
        public List<FlagData> flags;
    }
    //Recipe Progression Save Data
    [System.Serializable]
    private class RecipeProgressionData
    {
        public string recipeId;
        public int recipeCount;
    }
    //Specialization Progression Save Data
    [System.Serializable]
    private class SpecializationProgressionData
    {
        public string specializationName;
        public int specializationExp;
    }
    //Quests Save Data
    [System.Serializable]
    private class QuestProgressionData
    {
        public string questId;
        public List<int> tasksProgression; //How far along we are for each step, corresponds to index (use 0/1 for true/false completed status)
    }

    //Flags Save Data
    [System.Serializable]
    private class FlagData
    {
        public string flagName;
        public bool flagState;      
    }
    
    public string NewProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        saveData.knownSpecializations = new List<SpecializationProgressionData>();
        saveData.knownRecipes = new List<RecipeProgressionData>();
        saveData.activeQuests = new List<QuestProgressionData>();
        saveData.completedQuestIds = new List<string>();
        saveData.flags = new List<FlagData>();
        //Load all flags as false
        for (int i = 0; i < knownFlags.Count; i++)
        {
            FlagData f = new FlagData();
            f.flagName = knownFlags[i];
            f.flagState = false;
            saveData.flags.Add(f);
            flags[knownFlags[i]] = false; //Add it to the active data too
        }
        return JsonUtility.ToJson(saveData);      
    }

    public string GetProgressionData()
    {
        ProgressionData saveData = new ProgressionData();
        //Serialize specialization progression
        saveData.knownSpecializations = new List<SpecializationProgressionData>();
        Dictionary<string, int>.KeyCollection knownSpecializationKeys = knownSpecializations.Keys;
        foreach (string s in knownSpecializationKeys)
        {
            SpecializationProgressionData sp = new SpecializationProgressionData();
            sp.specializationName = s;
            sp.specializationExp = knownSpecializations[s];
            saveData.knownSpecializations.Add(sp);       
        }
        //Serialize recipe progression
        saveData.knownRecipes = new List<RecipeProgressionData>();
        Dictionary<string, int>.KeyCollection knownRecipeKeys = knownRecipes.Keys;
        foreach (string s in knownRecipeKeys)
        {
            RecipeProgressionData r = new RecipeProgressionData();
            r.recipeId = s;
            r.recipeCount = knownRecipes[s];
            saveData.knownRecipes.Add(r);
        }
        //Serialize Quest Data
        saveData.activeQuests = new List<QuestProgressionData>();
        //Dictionary<string, ActiveQuest>.KeyCollection activeQuestKeys = activeQuests.Keys;
        for (int i = 0; i < activeQuests.Count; i++)
        {
            QuestProgressionData q = new QuestProgressionData();
            q.questId = activeQuests[i].questId;
            q.tasksProgression = activeQuests[i].GetTaskSaveData();
            saveData.activeQuests.Add(q);
        }
        saveData.completedQuestIds = new List<string>(completedQuests);
        //Serialize flags
        saveData.flags = new List<FlagData>();
        Dictionary<string, bool>.KeyCollection flagKeys = flags.Keys;
        foreach (string s in flagKeys)
        {
            FlagData f = new FlagData();
            f.flagName = s;
            f.flagState = flags[s];
            saveData.flags.Add(f);       
        }
        return JsonUtility.ToJson(saveData);
    }
    
    public void SetProgressionData(string saveData)
    {
        ProgressionData loadedData = JsonUtility.FromJson<ProgressionData>(saveData);
        knownSpecializations.Clear();
        knownRecipes.Clear();
        activeQuests.Clear();
        for (int i = 0; i < questPanel.transform.childCount; i++)
        {
            Destroy(questPanel.transform.GetChild(i).gameObject);
        }
        if (saveData == "") { return; }
        //Set specialization data
        for (int i = 0; i < loadedData.knownSpecializations.Count; i++)
        {
            knownSpecializations[loadedData.knownSpecializations[i].specializationName] = loadedData.knownSpecializations[i].specializationExp;       
        }
        //Set recipe data
        for (int i = 0; i < loadedData.knownRecipes.Count; i++)
        {
            knownRecipes[loadedData.knownRecipes[i].recipeId] = loadedData.knownRecipes[i].recipeCount;
        }
        //Set quest data
        for (int i = 0; i < loadedData.activeQuests.Count; i++)
        {
            ActiveQuest q = ActivateQuest(loadedData.activeQuests[i].questId);
            q.SetTaskData(loadedData.activeQuests[i].tasksProgression);
        }
        completedQuests = new List<string>(loadedData.completedQuestIds);
        //Set flag data
        for (int i = 0; i < loadedData.flags.Count; i++)
        {
            flags[loadedData.flags[i].flagName] = loadedData.flags[i].flagState;       
        }
    }

    public void SaveMadeRecipe(string r)
    {
        if (!knownSpecializations.ContainsKey("baker")) knownSpecializations["baker"] = 0;
        if (isRecipeKnown(r))
        {
            knownSpecializations["baker"] += DataManager.instance.recipeManifest[r].expGiven;
            knownRecipes[r]++;
        }
        else
        {
            knownSpecializations["baker"] += DataManager.instance.recipeManifest[r].expGiven * 2;
            knownRecipes[r] = 1;
        }
        DataManager.instance.uiManager.UpdateUIVisuals();
    }
    
    public bool isRecipeKnown(string r)
    {
        return knownRecipes.ContainsKey(r);    
    }

    // QUESTS //
    
    public ActiveQuest ActivateQuest(string questId)
    {
        //Is this quest already complete?
        if (completedQuests.Contains(questId))
            return null;
        //Is this quest already active?
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questId == questId)
                return activeQuests[i];
        }
        //Find the quest id in the manifest
        QuestData q = DataManager.instance.questManifest[questId];
        //Create a list of serializable tasks for this quest, and provide default values
        ActiveQuest newQuest = new ActiveQuest();
        newQuest.questId = questId;
        for (int i = 0; i < q.questTasks.Count; i++)
        {
            ActiveQuestTaskData newTask = new ActiveQuestTaskData(q.questTasks[i]);
            newQuest.tasks.Add(newTask);
        }
        //Return the newly created activequest item
        GameObject hint = Instantiate(questHintPrefab, questPanel.transform, questPanel.transform);
        newQuest.activeQuestUIObject = hint.GetComponent<QuestHintDisplay>();
        newQuest.activeQuestUIObject.UpdateActiveQuestHint(newQuest);
        activeQuests.Add(newQuest);
        return newQuest;
    }

    public void UpdateQuest(ActiveQuest quest)
    {
        //If we still have tasks uncompleted, then just update the visual
        for (int i = 0; i < quest.tasks.Count; i++)
        {
            if (!quest.tasks[i].completed)
            {
                quest.activeQuestUIObject.UpdateActiveQuestHint(quest);
                return;
            }
        }
        //If all tasks are completed, then we should mark the quest complete
        CompleteQuest(quest);
    }

    public void CompleteQuest(ActiveQuest quest)
    {
        //Give the player the rewards
        List<QuestReward> rewardsToGive = DataManager.instance.questManifest[quest.questId].questRewards;
        for (int i = 0; i < rewardsToGive.Count; i++)
        {
            QuestReward reward = rewardsToGive[i];
            switch(reward.rewardType)
            {
                // MONEY //
                case (rewardType.Money):
                    DataManager.instance.AddMoney(reward.rewardQuantity);
                    break;
                // QUEST //
                case (rewardType.Quest):
                    ActivateQuest(reward.rewardId);
                    break;
                // FLAG //
                case (rewardType.Flag):
                    flags[reward.rewardId] = true;
                    break;
            }

        }
        //Update the quest visuals
        quest.activeQuestUIObject.UpdateActiveQuestHint(quest, true);
        //Remove from the active list and add to the completed list
        completedQuests.Add(quest.questId);
        activeQuests.Remove(quest);
    }

    public void QuestSignal(taskType taskType, string itemId, int quantity)
    {
        bool dataChanged = false;
        //For each task that exists in our active quests
        for (int i = 0; i < activeQuests.Count; i++)
        {
            for (int j = 0; j < activeQuests[i].tasks.Count; j++)
            {
                if (activeQuests[i].tasks[j].completed)
                    continue;
                //Are we paying attention to this task type?
                if (activeQuests[i].tasks[j].taskType == taskType)
                {
                    ActiveQuestTaskData relevantTask = activeQuests[i].tasks[j];
                    //Now our reaction is based on the task type (might not need this???)
                    switch(taskType)
                    {
                        case (taskType.CollectItem):
                        case (taskType.WaterCrop):
                        case (taskType.PlaceItem):
                        case (taskType.HarvestCrop):
                        case (taskType.InteractItem):
                            if (itemId == relevantTask.requiredItemId) //Is this the item we're looking for?
                            {
                                relevantTask.currentQuantity += quantity; //Update the quantity
                                dataChanged = true;
                                if (relevantTask.currentQuantity >= relevantTask.requiredQuantity)
                                {
                                    relevantTask.completed = true;
                                }    
                            }
                            break;
                        case (taskType.StockGood):
                            dataChanged = true;
                            if (DataManager.instance.store.goodsManifest.ContainsKey(relevantTask.requiredItemId))
                            {
                                relevantTask.currentQuantity = DataManager.instance.store.goodsManifest[relevantTask.requiredItemId];
                                if (relevantTask.currentQuantity >= relevantTask.requiredQuantity)
                                {
                                    relevantTask.completed = true;
                                } 
                            }
                            else
                            {
                                relevantTask.currentQuantity = 0;
                            }
                            break;
                    }   
                }
            }
            if (dataChanged)
                UpdateQuest(activeQuests[i]);
        }
    }
}

public class ActiveQuest
{
    public string questId;
    public List<ActiveQuestTaskData> tasks = new List<ActiveQuestTaskData>();
    public QuestHintDisplay activeQuestUIObject = null;
    public List<int> GetTaskSaveData()
    {
        List<int> progressToSave = new List<int>();
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].requiredQuantity != 0) //If we have a required quantity, then we are tracking a value
            {
                progressToSave.Add(tasks[i].currentQuantity);
            } else //if we don't then figure out if this task is completed
            {
                if (tasks[i].completed)
                    progressToSave.Add(1);
                else
                    progressToSave.Add(0);
            }
        }
        return progressToSave;
    }
    public void SetTaskData(List<int> progressData)
    {
        for (int i = 0; i < progressData.Count; i++)
        {
            if (tasks[i].requiredQuantity != 0) //If we have a required quantity, then we are tracking a value
            {
                tasks[i].currentQuantity = progressData[i];
                if (tasks[i].currentQuantity >= tasks[i].requiredQuantity)
                {
                    tasks[i].completed = true;
                } else
                    tasks[i].completed = false;
            } else //if we don't then figure out if this task is completed
            {
                if (progressData[i] == 0)
                    tasks[i].completed = false;
                else
                    tasks[i].completed = true;
            }
        }
        if (activeQuestUIObject != null)
            activeQuestUIObject.UpdateActiveQuestHint(this);
    }
}
public class ActiveQuestTaskData
{
    public taskType taskType;
    public int requiredQuantity;
    public int currentQuantity;
    public string requiredItemId;
    public bool completed;
    public ActiveQuestTaskData(QuestTask q)
    {
        taskType = q.taskType;
        requiredQuantity = q.taskQuantity;
        currentQuantity = 0;
        requiredItemId = q.taskItemId;
        completed = false;
    }
}
