using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_NPC : NPCState
{
    private NPC npc;
    public IdleState_NPC(NPC npcBase, NPCStateMachine stateMachine, string animBoolName) : base(npcBase, stateMachine, animBoolName)
    {
        npc = npcBase;
    }

    public override void Enter()
    {
        Debug.Log("Entered Idle State");
        base.Enter();
        stateTimer = npc.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("Updating Idle State, time left: " + stateTimer);
        if (stateTimer < 0f)
        {
            stateMachine.ChangeState(npc.moveState);
        }
    }
}
