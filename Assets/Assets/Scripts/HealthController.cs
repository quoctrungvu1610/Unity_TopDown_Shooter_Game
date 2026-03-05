using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage) 
    {
        currentHealth-= damage;
        if(currentHealth <= 0) 
        {
            currentHealth = 0;
            ShouldDie();
        }

    }
    public virtual void IncreaseHealth() 
    {
        currentHealth++;

        if(currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }
    }

    public bool ShouldDie() 
    {
        return currentHealth <= 0;
    }
}
