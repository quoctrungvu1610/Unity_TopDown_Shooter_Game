using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] List<string> objectives = new List<string>();

    public string GetTitle() 
    {
        return name;
    }

    public int GetObjectiveCount() 
    {
        return objectives.Count;
    }

    public IEnumerable<string> GetObjectives() 
    {
        return objectives;
    }

    public bool HasObject(string objective)
    {
        return objectives.Contains(objective);
    }

    public static Quest GetByName(string questName) 
    {
        foreach (Quest quest in Resources.LoadAll<Quest>("")) 
        {
            Debug.Log(quest.name);
            if (quest.name == questName) 
            {
                return quest;
            }
        
        }
        return null;
    }
}
