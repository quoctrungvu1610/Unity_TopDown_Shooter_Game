using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [SerializeField] List<DialogueNode> nodes;

#if UNITY_EDITOR
    private void Awake()
    {
        if(nodes.Count == 0)
        {
            Debug.LogWarning("Dialogue has no nodes, adding empty list of nodes to prevent null reference exceptions.");
            nodes.Add(new DialogueNode());
        }
    }

#endif

    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }
}
