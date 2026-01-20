using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shield : MonoBehaviour
{
    [SerializeField] private int durability;
    private Enemy_Melee enemyMelee;

    private void Awake()
    {
        enemyMelee = GetComponentInParent<Enemy_Melee>();
    }
    public void ReduceDurability() 
    {
        durability--;
        if(durability <= 0) 
        {
            enemyMelee.anim.SetFloat("ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }
}
