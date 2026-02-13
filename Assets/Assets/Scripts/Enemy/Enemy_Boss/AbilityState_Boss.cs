using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private bool isEvenCalled = false;
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();
        isEvenCalled = false;
        stateTimer = enemy.flamethrowDuration;

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetAbilityOnCooldown();
        enemy.bossVisual.ResetBatteries();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (ShouldDisabledFlamethrower() && isEvenCalled == false)
        {
            Debug.Log("Disable Flamethrower from Ability State");
            DisableFlamethrower();
            isEvenCalled = true;
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    private bool ShouldDisabledFlamethrower()
    {
        return stateTimer < 0;
    }

    public void DisableFlamethrower()
    {
        if (enemy.bossWeaponType == BossWeaponType.Hammer) 
        {
            return;
        }

        if (enemy.flamethrowActive == false) 
        {
            return;
        }
        enemy.ActivateFlamethrower(false, true);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.Flamethrower) 
        {
            //int randomAbility = Random.Range(0, 2);
            int randomAbility = 1;
            if (randomAbility == 0)
            {
                enemy.ActivateFlamethrower(true, true);
            }
            else
            {
                enemy.FireRainMissile();
                enemy.ActivateFlamethrower(true, false);
            }
            enemy.bossVisual.DischargeBatteries();
        }
        if (enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
        }
    }
}
