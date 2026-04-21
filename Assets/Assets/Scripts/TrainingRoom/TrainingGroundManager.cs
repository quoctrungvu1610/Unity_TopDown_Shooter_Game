using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGroundManager : MonoBehaviour, ISwitchable
{
    [SerializeField] private TrainingDummySpawner[] dummySpawners;
    [SerializeField] private float minTimeBetweenSpawner = 2f;
    [SerializeField] private float maxTimeBetweenSpawner = 5f;

    private bool isOn = false;

    public void StartTrainingGround() 
    {
        StartCoroutine(StartSpawnDummies());
    }

    public IEnumerator StartSpawnDummies() 
    {
        foreach (var spawner in dummySpawners) 
        {
            spawner.StartSpawnDummy();
            yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawner, maxTimeBetweenSpawner));
        }
    }

    public void StopTrainingGround() 
    {
        foreach (var spawner in dummySpawners) 
        {
            spawner.StopSpawnDummy();
        }
    }

    public void Switch()
    {
        isOn = !isOn;
        if(isOn) 
        {
            StartTrainingGround();
        } else 
        {
            StopTrainingGround();
        }
    }
}
