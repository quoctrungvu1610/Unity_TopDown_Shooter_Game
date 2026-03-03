using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shield : MonoBehaviour, IDamageable
{
    [SerializeField] private int durability;
    private Enemy_Melee enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();

        durability = enemy.shieldDurability;
    }
    public void ReduceDurability() 
    {
        durability--;
        if(durability <= 0) 
        {
            enemy.anim.SetFloat("ChaseIndex", 0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        ReduceDurability();
    }
}
