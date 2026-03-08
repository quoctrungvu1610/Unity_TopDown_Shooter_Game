using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform objectiveContainer;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject objectIncompletePrefab;

    public void Setup(QuestStatus status) 
    {
        Quest quest = status.GetQuest();
        title.text = quest.GetTitle();
        objectiveContainer.DetachChildren();

        foreach(string objective in quest.GetObjectives()) 
        {
            GameObject prefab = status.IsObjectiveCompleted(objective) ? objectivePrefab : objectIncompletePrefab;
            GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
            TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
            objectiveText.text = objective;
        }
    }
}
