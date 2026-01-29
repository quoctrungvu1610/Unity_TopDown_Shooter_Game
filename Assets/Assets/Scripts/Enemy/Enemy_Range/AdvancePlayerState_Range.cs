using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPos;

    public float lastTimeAdvanced { get; private set; }
    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true, true);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;

        if (enemy.IsUnstoppable()) 
        {
            enemy.visuals.EnableIK(true, false);
            stateTimer = enemy.advanceDuration;
        }
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAdvanced = Time.time;
    }

    public override void Update()
    {
        base.Update();

        playerPos = enemy.player.position;
        enemy.UpdateAimPosition();
        enemy.agent.SetDestination(playerPos);
        enemy.FaceTarget(GetNextPathPoint());

        if (CanEnterBattleState() && enemy.IsSeeingPlayer()) 
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    private bool CanEnterBattleState() 
    {
        bool closeEnoughToPlayer = Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance;
        if (enemy.IsUnstoppable())
        {
            return closeEnoughToPlayer || stateTimer < 0;
        }
        else 
        {
            return closeEnoughToPlayer;
        }
    }
}
