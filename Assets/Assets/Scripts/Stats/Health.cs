using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float startHealth;
    public float currentHealth;
    public bool isDead { get; private set; }

    public virtual void Awake()
    {
        
    }

    public virtual void Start()
    {
        Redraw();  
    }

    public virtual void Redraw() 
    {
        
    }

    public virtual void TakeDamage(float damage) 
    {
        currentHealth -= damage;
        if(currentHealth <= 0) 
        {
            Die();
            currentHealth = 0;
        }
    }

    public virtual void IncreaseHealth(float health) 
    {
        
    }

    public virtual void Die() 
    {
        isDead = true;

    }
}
