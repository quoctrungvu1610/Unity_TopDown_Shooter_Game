using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DummyDirection { LeftToRight, RightToLeft }

public class TrainingDummySpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float minTimeBetweenWaves = 5f;
    [SerializeField] private float maxTimeBetweenWaves = 10f;

    [SerializeField] private int maxDummyPerWave = 10;
    [SerializeField] private int minDummyPerWave = 10;
    [SerializeField] private int numberOfDummies = 5;

    [SerializeField] private float delayBetweenDummies = 0.5f;
    [SerializeField] private float minDelayBetweenDummies = 0.5f;
    [SerializeField] private float maxDelayBetweenDummies = 2f;

    [SerializeField] private float dummyMoveTime = 5f;
    [SerializeField] private float minDummyMoveTime = 5f;
    [SerializeField] private float maxDummyMoveTime = 10f;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private Vector3 spawnPoint;
    private Vector3 targetPoint;

    private DummyDirection dummyDirection = DummyDirection.LeftToRight;

    private bool spawnNextWave = true;

    [SerializeField] private TrainingDummy dummyPrefab = null;


    public void StartSpawnDummy() 
    {
        spawnNextWave = true;
        SetupNewWave();
    }

    public void StopSpawnDummy() 
    {
        spawnNextWave = false;
    }


    private void SetupNewWave() 
    {
        numberOfDummies = Random.Range(minDummyPerWave, maxDummyPerWave + 1);
        dummyDirection = (DummyDirection)Random.Range(0, 2);
        dummyMoveTime = Random.Range(minDummyMoveTime, maxDummyMoveTime);
        delayBetweenDummies = Random.Range(minDelayBetweenDummies, maxDelayBetweenDummies);

        SetupPosition(dummyDirection);
        StartCoroutine(SpawnDummies());

    }

    private IEnumerator SpawnDummies() 
    {
        for (int i = 0; i < numberOfDummies; i++) 
        {
            //TrainingDummy newDummy =  Instantiate(dummyPrefab, spawnPoint, Quaternion.LookRotation(Vector3.left));
            GameObject newDummy = ObjectPool.instance.GetObject(dummyPrefab.gameObject, transform);
            newDummy.transform.position = spawnPoint;
            newDummy.transform.rotation = Quaternion.LookRotation(Vector3.left);
            newDummy.GetComponent<TrainingDummy>().Setup(dummyMoveTime, spawnPoint, targetPoint);
            newDummy.GetComponent<TrainingDummy>().StartMoveDummy();
            yield return new WaitForSeconds(delayBetweenDummies);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        if (spawnNextWave) 
        {
            SetupNewWave();
        }
    }

    private void SetupPosition(DummyDirection dummyDirection)
    {
        if (dummyDirection == DummyDirection.LeftToRight) 
        {
            spawnPoint = leftPoint.position;
            targetPoint = rightPoint.position;
        } 
        else 
        {
            spawnPoint = rightPoint.position;
            targetPoint = leftPoint.position;
        }
    }
}
