using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : HealthController
{
    public Healthbar healthBar;

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);
        healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    public override IEnumerator RemoveHealthBar()
    {
        yield return new WaitForSeconds(0.5f);
        healthBar.gameObject.SetActive(false);
    }

}
