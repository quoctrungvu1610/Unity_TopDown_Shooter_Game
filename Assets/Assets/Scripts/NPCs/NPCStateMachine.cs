using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine
{
    public NPCState currentState { get; private set; }

    public void Initialize(NPCState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(NPCState newState) 
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
