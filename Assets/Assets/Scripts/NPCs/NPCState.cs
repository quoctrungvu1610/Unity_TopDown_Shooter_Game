using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCState
{
    protected NPC npcBase;
    protected NPCStateMachine stateMachine;
    protected string animBoolName;

    protected float stateTimer;

    public NPCState(NPC npcBase, NPCStateMachine stateMachine, string animBoolName) 
    {
        this.npcBase = npcBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() 
    {
        npcBase.anim.SetBool(animBoolName, true);
    }
    public virtual void Update() 
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit() 
    {
        npcBase.anim.SetBool(animBoolName, false);
    }

    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = npcBase.agent;
        NavMeshPath path = agent.path;
        if (path.corners.Length < 2)
        {
            return agent.destination;
        }

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
            {
                return path.corners[i + 1];
            }
        }
        return agent.destination;
    }

}
