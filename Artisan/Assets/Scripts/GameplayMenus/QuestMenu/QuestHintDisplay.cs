using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuestHintDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questTitle;
    [SerializeField] Transform taskHintPanel;
    [SerializeField] GameObject taskHintDisplayPrefab;
    public void UpdateActiveQuestHint(ActiveQuest questData, bool completed = false)
    {
        //If we are here because we finished the quest, just destroy this notification
        if (completed)
            Destroy(gameObject);

        questTitle.text = DataManager.instance.questManifest[questData.questId].questTitle;
        for (int i = 0; i < taskHintPanel.childCount; i++)
        {
            Destroy(taskHintPanel.GetChild(i).gameObject);
        }
        float height = 60f; //Calculate the height of this panel
        List<QuestTask> tasks = DataManager.instance.questManifest[questData.questId].questTasks;
        for (int i = 0; i < tasks.Count; i++)
        {
            GameObject task = Instantiate(taskHintDisplayPrefab, taskHintPanel, taskHintPanel);
            height += 35f; //Again, height calculation, super gross
            string display = tasks[i].description + " ";
            if (tasks[i].taskQuantity != 0) 
            { 
                display += "(" + questData.tasks[i].currentQuantity.ToString() + "/" + questData.tasks[i].requiredQuantity.ToString() + ")"; 
            }
            task.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = display;
            if (questData.tasks[i].completed)
            {
                task.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            } else
            {
                task.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            }
        }
        //Set the height of the display in the grossest way possible
        RectTransform r = GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(r.rect.width, height);
    }
}
