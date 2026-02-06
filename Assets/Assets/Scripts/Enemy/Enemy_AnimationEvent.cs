using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationEventp : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Melee enemyMelee;
    private Enemy_Boss enemyBoss;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
        enemyBoss = GetComponentInParent<Enemy_Boss>();
    }

    public void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    public void StartManualMovement()
    {
        enemy.ActivateManualMovement(true);
    }

    public void StopManualMovement()
    {
        enemy.ActivateManualMovement(false);
    }

    public void StartManualRotation()
    {
        enemy.ActivateManualRotation(true);
    }

    public void StopManualRotation()
    {
        enemy.ActivateManualRotation(false);
    }

    public void AbilityEvent() 
    {
        enemy.AbilityTrigger();
    }

    public void EnableIK() 
    {
        enemy.visuals.EnableIK(true, true, 1.5f);
    }

    public void EnableWeaponModel() 
    {
        enemy.visuals.EnableWeaponModel(true);
        enemy.visuals.EnableSecondaryWeaponModel(false);
    }

    public void BossJumpImpact() 
    {
        enemyBoss?.JumpImpact();
    }

    public void BeginMeleeAttackCheck() 
    {
        enemyMelee?.EnableAttackCheck(true);
    }

    public void FinishMeleeAttackCheck()
    {
        enemyMelee?.EnableAttackCheck(false);
    }
}
