using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationEventp : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Boss enemy_Boss;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
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
        if (enemy_Boss == null) 
        {
            enemy_Boss = GetComponentInParent<Enemy_Boss>();
        }

        enemy_Boss?.JumpImpact();

    }
}
