using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
    Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
    [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);

#if UNITY_EDITOR
    private void Awake()
    {
        OnValidate();
    }

#endif

    private void OnValidate()
    {


        nodeLookup.Clear();

        foreach (DialogueNode node in GetAllNodes()) 
        {
            nodeLookup[node.name] = node;
        }
    }

    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }

    public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
        foreach (string childID in parentNode.GetChildren())
        {
            if (nodeLookup.ContainsKey(childID))
            {
                yield return nodeLookup[childID];
            }
        }
    }

    public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
    {
        foreach (DialogueNode node in GetAllChildren(currentNode))
        {
            if (node.IsPlayerSpeaking())
            {
                yield return node;
            }
        }
    }

    public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
    {
        foreach (DialogueNode node in GetAllChildren(currentNode))
        {
            if (!node.IsPlayerSpeaking())
            {
                yield return node;
            }
        }
    }

#if UNITY_EDITOR
    public void CreateNode(DialogueNode parent)
    {
        DialogueNode newNode = MakeNode(parent);
        Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
        Undo.RecordObject(newNode, "Added Dialogue Node");
        AddNode(newNode);
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        nodes.Remove(nodeToDelete);
        CleanDanglingChildren(nodeToDelete);
        OnValidate();
        Undo.DestroyObjectImmediate(nodeToDelete);

    }

    private void AddNode(DialogueNode newNode)
    {
        nodes.Add(newNode);
        OnValidate();
    }

    private DialogueNode MakeNode(DialogueNode parent)
    {
        DialogueNode newNode = CreateInstance<DialogueNode>();
        newNode.name = System.Guid.NewGuid().ToString();

        if (parent != null)
        {
            parent.AddChild(newNode.name);
            newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
            newNode.SetPosition(parent.GetRect().position + newNodeOffset);
        }

        return newNode;
    }

    private void CleanDanglingChildren(DialogueNode nodeToDelete)
    {
        foreach (DialogueNode node in GetAllNodes())
        {
            node.RemoveChild(nodeToDelete.name);
        }
    }
#endif

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (nodes.Count == 0)
        {
            DialogueNode newNode = MakeNode(null);
            AddNode(newNode);
        }

        if (AssetDatabase.GetAssetPath(this) != "") 
        {
            foreach (DialogueNode node in GetAllNodes()) 
            {
                if(AssetDatabase.GetAssetPath(node) == "") //If the node is not already an asset, add it to the asset database as a sub-asset of the dialogue. This ensures that the node will be saved with the dialogue and will not be lost when the dialogue is saved.
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }
#endif
    }

    public void OnAfterDeserialize()
    {
       
    }
}
