using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCompletion : MonoBehaviour
{
    [SerializeField] Quest quest;
    [SerializeField] string objective;

    public void CompleteObective() 
    {
        Debug.Log($"Completing quest {quest.GetTitle()} objective {objective}");
        QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        questList.CompleteObjective(quest, objective);
    }
}
