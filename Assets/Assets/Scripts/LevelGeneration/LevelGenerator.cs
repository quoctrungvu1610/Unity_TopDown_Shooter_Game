using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    [SerializeField] private List<Transform> generatedLevelParts = new List<Transform>();
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;


    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer = 0f;
    private bool generationOver;

    private void Start()
    {
        defaultSnapPoint = nextSnapPoint;
        InitializeGeneration();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0f) 
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (generationOver == false) 
            {
                FinishGeneration();
            }
        }
    }

    [ContextMenu("Restart Generation")]
    private void InitializeGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);
        DestroyOldLevelParts();
    }

    private void DestroyOldLevelParts()
    {
        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }
        generatedLevelParts = new List<Transform>();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();
    }

    [ContextMenu("Create Next Level Part")]
    private void GenerateNextLevelPart() 
    {
        Transform newPart = null;
        if (generationOver)
        {
            newPart = Instantiate(lastLevelPart);
        }
        else 
        {
            newPart = Instantiate(ChooseRandomPart());
        }

        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if(levelPartScript.InterSectionDetected()) 
        {
            InitializeGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitPoint();
    }
    

    private Transform ChooseRandomPart() 
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform choosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return choosenPart;
    }
}
