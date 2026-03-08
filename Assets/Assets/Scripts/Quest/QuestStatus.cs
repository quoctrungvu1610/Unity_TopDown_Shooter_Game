using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStatus
{
     [SerializeField] Quest quest;
     [SerializeField] List<string> completedObjectives = new List<string>();


    [System.Serializable]
    class QuestStatusRecord 
    {
        public string questName;
        public List<string> completedObjectives;
    }

    public QuestStatus(Quest quest) 
    {
        this.quest = quest;
    }

    public QuestStatus(object objectState)
    {
        QuestStatusRecord state = objectState as QuestStatusRecord;
        quest = Quest.GetByName(state.questName);
        completedObjectives = state.completedObjectives;
    }

    public Quest GetQuest() 
    {
        return quest;
    }

    public int GetCompletedCount() 
    {
        return completedObjectives.Count;

    }

    public bool IsObjectiveCompleted(string objective) 
    {
        return completedObjectives.Contains(objective);
    }

    public void CompeteObjective(string objective)
    {
        if (quest.HasObject(objective)) 
        {
            completedObjectives.Add(objective);
        }
    }

    public object CaptureState()
    {
        QuestStatusRecord state = new QuestStatusRecord();
        state.questName = quest.name;
        state.completedObjectives = completedObjectives;

        return state;
    }
}
