using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;
    private float damageCooldown;
    private float lastTimeDamaged;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemy.flamethrowActive == false)
        {
            return;
        }

        if(Time.time - lastTimeDamaged < damageCooldown) 
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        { 
            damageable?.TakeDamage();
            lastTimeDamaged = Time.time;
            damageCooldown = enemy.flameDamageCooldown;
        }
    }

}
