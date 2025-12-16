using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuestHintDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questTitle;
    [SerializeField] Transform taskHintPanel;
    [SerializeField] GameObject taskHintDisplayPrefab;
    public void UpdateActiveQuestHint(ActiveQuest questData)
    {
        questTitle.text = DataManager.instance.questManifest[questData.questId].questTitle;
        for (int i = 0; i < taskHintPanel.childCount; i++)
        {
            Destroy(taskHintPanel.GetChild(i).gameObject);
        }
        List<QuestTask> tasks = DataManager.instance.questManifest[questData.questId].questTasks;
        for (int i = 0; i < tasks.Count; i++)
        {
            GameObject task = Instantiate(taskHintDisplayPrefab, taskHintPanel, taskHintPanel);
            string display = tasks[i].description + " ";
            if (tasks[i].taskQuantity != 0) 
            { 
                display += "(" + questData.tasks[i].currentQuantity.ToString() + "/" + questData.tasks[i].requiredQuantity.ToString() + ")"; 
            }
            task.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = display;
        }
    }
}
