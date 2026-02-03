using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss;
    }

    public override void Enter()
    {
        base.Enter();

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

        if (ShouldDisabledFlamethrower())
        {
            DisableFlamethrower();
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
        if (enemy.bossWeaponType == BossWeaponType.Flamethrower) 
        {
            return;
        }

        if (enemy.flamethrowActive == false) 
        {
            return;
        }
        enemy.ActivateFlamethrower(false);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.bossWeaponType == BossWeaponType.Flamethrower) 
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisual.DischargeBatteries();
        }
        if (enemy.bossWeaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
        }
    }
}
