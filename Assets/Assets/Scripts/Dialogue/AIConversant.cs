using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIConversant : Interactable
{
    [SerializeField] private string npcName;
    [SerializeField] private Dialogue dialogue;
    
    private PlayerConversant playerConversant;

    protected override void Awake()
    {
        base.Awake();
        playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
    }

    public string GetNPCName() 
    {
        return npcName;
    }

    override public void Interaction()
    {
        base.Interaction();

        Debug.Log("Starting conversation with " + gameObject.name);

        if(playerConversant == null) 
        {
            Debug.LogWarning("No PlayerConversant found on player");
            return;
        }

        if (dialogue == null)
        {
            Debug.LogWarning("No dialogue assigned to " + gameObject.name);
            return;
        }
        else 
        {
            playerConversant.StartDialogue(this, dialogue);
        }

    }
}
