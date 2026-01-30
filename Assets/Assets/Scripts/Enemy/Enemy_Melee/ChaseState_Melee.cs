using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastTimeUpdatedDestination;
    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableWeaponModel(true); 

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange()) 
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination()) 
        {
            enemy.agent.destination = enemy.player.position;
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time >= lastTimeUpdatedDestination + 0.2f)
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }
        return false;
    }
}
