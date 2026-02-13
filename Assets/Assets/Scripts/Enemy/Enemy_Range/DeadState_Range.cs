using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy;
    private bool interactionDisabled;

    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.finishedThrowingGrenade == false && enemy.grenadePerk == GrenadePerk.CanThrowGrenade) 
        {
            enemy.ThrowGrenade();
        }

        interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    //public override void Exit()
    //{
    //    base.Exit();
    //}

    public override void Update()
    {
        base.Update();

        //DisableInteractionIfShould();

    }
    private void DisableInteractionIfShould()
    {
        if (stateTimer < 0f && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.CollidersActive(false);
        }
    }

}
