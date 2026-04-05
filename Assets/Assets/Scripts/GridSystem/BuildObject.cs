using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    [SerializeField] private BuildObjectType buildObjectType;

    public BuildObjectType GetBuildObjectType() 
    {
        return buildObjectType;
    }
}
