using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "New Progression", menuName = "Progression/New Progression")]
public class Progression : ScriptableObject
{
    public List<PlayerProgress> progress = new List<PlayerProgress>();


    [System.Serializable]
    public class PlayerProgress 
    {   
        public Stat stat;
        public List<float> statData = new List<float>();
    
    }

    public IEnumerable<PlayerProgress> GetProgresses() 
    {
        return progress;
    }

    public List<float> GetBaseStatDatas(Stat stat) 
    {
        foreach (PlayerProgress progress in progress) 
        {
            if (progress.stat == stat) 
            {
                return progress.statData;
            }
        }
        return null;
    }

    public float GetBaseStatDataByLevel(Stat stat, float level) 
    {
        List<float> statDatas = GetBaseStatDatas(stat);
        if (statDatas != null) 
        {
            for (int i = 0; i < statDatas.Count; i++) 
            {
                if (i == level - 1) 
                {
                    return statDatas[i];
                }
            }
        }
        return 0;
    }

}
