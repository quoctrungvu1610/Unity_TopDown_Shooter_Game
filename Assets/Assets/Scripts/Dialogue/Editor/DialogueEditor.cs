using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    Dialogue selectedDialogue = null;
    GUIStyle nodeStyle;

    //Dragging
    bool dragging = false;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
        //utility parameters: type of window, utility, title of window
        GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");

    }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        if (dialogue != null)
        {
            ShowEditorWindow();
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(20, 20, 20, 20);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    private void OnSelectionChanged()
    {
        Dialogue newDialogue = Selection.activeObject as Dialogue;
        if (newDialogue != null)
        {
            selectedDialogue = newDialogue;
            Repaint();
        }
    }

    private void OnGUI()
    {

        if (selectedDialogue == null)
        {
            EditorGUILayout.LabelField("No Dialogue Selected.");
        }
        else
        {
            ProcessEvent();
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                OnGUINode(node);
            }
        }

    }

    private void ProcessEvent() 
    {
        if (Event.current.type == EventType.MouseDown && !dragging) 
        {
            dragging = true;
        }
        else if(Event.current.type == EventType.MouseDrag && dragging) 
        {
            Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
            selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;

            GUI.changed = true;

            //Also can use Repaint()
            //Repaint();
            
        }
        else if (Event.current.type == EventType.MouseUp && dragging)
        {
            dragging = false;
            selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
        }
    }

    private void OnGUINode(DialogueNode node)
    {
        GUILayout.BeginArea(node.rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
        string newText = EditorGUILayout.TextField(node.text);
        string neUniqueID = EditorGUILayout.TextField(node.uniqueID);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

            node.text = newText;
            node.uniqueID = neUniqueID;
            //NOTE: This is a bit of a hack to make sure that changes to the dialogue nodes are saved. Since DialogueNode is not a ScriptableObject, Unity doesn't automatically know when it has been modified and needs to be saved. By calling SetDirty on the parent Dialogue ScriptableObject, we can tell Unity that it has been modified and needs to be saved.
            //EditorUtility.SetDirty(selectedDialogue);
        }

        GUILayout.EndArea();
    }
}

