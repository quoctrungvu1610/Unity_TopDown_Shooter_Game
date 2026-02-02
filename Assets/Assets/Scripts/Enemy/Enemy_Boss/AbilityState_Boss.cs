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
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);
        if (stateTimer < 0 && enemy.flamethrowActive) 
        {
            enemy.ActivateFlamethrower(false);
        }

        if (triggerCalled) 
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ActivateFlamethrower(true);
    }
}
