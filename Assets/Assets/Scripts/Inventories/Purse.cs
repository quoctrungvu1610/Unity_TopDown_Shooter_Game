using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Purse : MonoBehaviour
{
    [SerializeField] private float startingBalane = 1000f;

    private float balance = 0;

    public event Action onchange;

    private void Awake()
    {
        balance = startingBalane;
    }

    public float GetBalance() 
    {
        return balance;
    }

    public void UpdateBalance(float amount) 
    {
        balance += amount;
        if (onchange != null) 
        {
            onchange();
        }
    }
}
