using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private Player player;
    private float bonusAddNumber;
    private float bonusPercentageNumber;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void Start()
    {
        base.Start();
        player.equipment.equipmentUpdated += Redraw;
        startHealth = player.stat.GetStat(Stat.Health);
        currentHealth = startHealth;

        Redraw();
    }

    public override void TakeDamage(float damage) 
    {
        base.TakeDamage(damage);
    }

    public override void Die() 
    {
        base.Die();

        player.anim.enabled = false;
        player.ragdoll.RagdollActive(true);
        player.aim.DisableLaser();
        player.movement.enabled = false;
        player.weapon.enabled = false;
    }

    public void OnAddEquipment()
    {
        float percentBonus = startHealth * (bonusPercentageNumber / 100f);

        currentHealth += bonusAddNumber + percentBonus;

        if (currentHealth > startHealth + bonusAddNumber + percentBonus)
        {
            currentHealth = startHealth + bonusAddNumber + percentBonus;
        }
    }

    public void OnRemoveEquipment()
    {
        float percentBonus = startHealth * (bonusPercentageNumber / 100f);
        currentHealth -= bonusAddNumber + percentBonus;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public override void Redraw()
    {
        base.Redraw();
        startHealth = player.stat.GetStat(Stat.Health);
    }

    public void UpdateCurrentEquipmentData(StatEquipableItem item) 
    {
        float addBonus = 0;
        foreach (float bonusNum in item.GetAdditiveModifiers(Stat.Health)) 
        {
            addBonus += bonusNum;
        }
        bonusAddNumber = addBonus;

        float percentageBonus = 0;
        foreach (float bonusNum in item.GetPercentageModifiers(Stat.Health))
        {
            percentageBonus += bonusNum;
        }
        bonusPercentageNumber = percentageBonus;
    }

    private void OnDestroy()
    {
        player.equipment.equipmentUpdated -= Redraw;
    }
}
