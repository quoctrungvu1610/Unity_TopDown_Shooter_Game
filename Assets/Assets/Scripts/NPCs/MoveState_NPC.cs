using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_NPC : NPCState
{
    private NPC npc;
    private Vector3 destination;
    public MoveState_NPC(NPC npcBase, NPCStateMachine stateMachine, string animBoolName) : base(npcBase, stateMachine, animBoolName)
    {
        npc = npcBase;
    }

    public override void Enter()
    {
        base.Enter();

        npc.agent.speed = npc.walkSpeed;
        destination = npc.GetPatrolDestination();
        npc.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        base.Update();

        npc.FaceTarget(GetNextPathPoint());

        if (npc.agent.remainingDistance <= npc.agent.stoppingDistance + 0.05f)
        {
            stateMachine.ChangeState(npc.idleState);
        }
    }
}
