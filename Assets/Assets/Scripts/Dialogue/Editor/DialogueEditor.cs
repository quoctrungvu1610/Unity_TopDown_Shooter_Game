using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    Dialogue selectedDialogue = null;

    [NonSerialized]
    GUIStyle nodeStyle;

    [NonSerialized]
    GUIStyle playerNodeStyle;

    [NonSerialized]
    DialogueNode draggingNode = null;

    [NonSerialized]
    Vector2 draggingOffset;

    [NonSerialized]
    DialogueNode creatingNode = null;

    [NonSerialized]
    DialogueNode deletingNode = null;

    [NonSerialized]
    DialogueNode linkingParentNode = null;



    Vector2 scrollPosition;

    [NonSerialized]
    bool draggingCanvas = false;

    [NonSerialized]
    Vector2 draggingCanvasOffset;

    const float canvasSize = 10000;
    const float backgroundSize = 50;



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
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(20, 20, 20, 20);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        playerNodeStyle = new GUIStyle();
        playerNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        playerNodeStyle.normal.textColor = Color.white;
        playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
        playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
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

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize); //Make the scroll view big enough to allow for scrolling in any direction. This is a bit of a hack, but it works. It allows for infinite scrolling in any direction, which is useful for large dialogues.
            Texture2D backgroundTex = Resources.Load("background") as Texture2D;
            Rect textCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize); //Make the texture repeat across the entire canvas. This is done by setting the texture coordinates to be larger than 1, which causes the texture to repeat. The number of times the texture repeats is determined by the size of the canvas divided by the size of the texture.
            GUI.DrawTextureWithTexCoords(canvas, backgroundTex, textCoords);

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnection(node);
            }
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
            }

            EditorGUILayout.EndScrollView();

            if (creatingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }
            if (deletingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }

    }

    private void DrawConnection(DialogueNode node)
    {
        Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
        foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node)) 
        {
            Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y); 
            Vector3 controlPointOffset = endPosition - startPosition;
            controlPointOffset.y = 0; //Make the control point offset only affect the x position, so that the curve is always horizontal. This makes it look nicer and easier to read.
            controlPointOffset.x *= 0.5f; //Make the control point offset smaller, so that the curve is less curved and easier to read.
            Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.white, null, 4f);
        }
    }

    private void ProcessEvent() 
    {
        if (Event.current.type == EventType.MouseDown && draggingNode == null)
        {
            draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
            if (draggingNode != null)
            {
                draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition; //Calculate the offset between the mouse position and the node position. This allows for smooth dragging of the node, as the node will move by the same amount as the mouse, rather than snapping to the mouse position.
                Selection.activeObject = draggingNode; //Select the node that is being dragged. This allows for easy editing of the node's properties in the inspector while it is being dragged.

            }
            else
            {
                draggingCanvas = true;
                draggingCanvasOffset = Event.current.mousePosition + scrollPosition; //Calculate the offset between the mouse position and the scroll position. This allows for smooth dragging of the canvas, as the canvas will move by the same amount as the mouse, rather than snapping to the mouse position.
                Selection.activeObject = selectedDialogue; //Select the dialogue that is being edited. This allows for easy editing of the dialogue's properties in the inspector while the canvas is being dragged.
            }
        }
        else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
        {
            draggingNode.SetPosition(Event.current.mousePosition + draggingOffset); //Move the node by the amount the mouse has moved since the last event. This allows for smooth dragging of the node.
            GUI.changed = true;
            //Repaint();
        }
        else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
        {
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition; //Move the scroll position by the amount the mouse has moved since the last event. This allows for smooth dragging of the canvas.
            GUI.changed = true;
        }

        else if (Event.current.type == EventType.MouseUp && draggingNode != null)
        {
            draggingNode = null;
        }
        else if (Event.current.type == EventType.MouseUp && draggingCanvas) 
        {
            draggingCanvas = false;
        }
    }

    private void DrawNode(DialogueNode node)
    {
        GUIStyle style = nodeStyle;
        if (node.IsPlayerSpeaking()) 
        {
            style = playerNodeStyle;
        }

        GUILayout.BeginArea(node.GetRect(), style);

        node.SetText(EditorGUILayout.TextField(node.GetText()));

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("+"))
        {
            creatingNode = node;
        }

        DrawLinkButtons(node);

        if (GUILayout.Button("x"))
        {
            deletingNode = node;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawLinkButtons(DialogueNode node)
    {
        if (linkingParentNode == null)
        {
            if (GUILayout.Button("Link"))
            {
                linkingParentNode = node;
            }
        }
        else if(linkingParentNode == node) 
        {
            if (GUILayout.Button("Cancel"))
            {
                linkingParentNode = null;
            }
        }
        else if (linkingParentNode.GetChildren().Contains(node.name)) 
        {
            if (GUILayout.Button("Unlink"))
            {
                linkingParentNode.RemoveChild(node.name);
                linkingParentNode = null;
            }
        }
        else
        {
            if (GUILayout.Button("Child"))
            {
                linkingParentNode.AddChild(node.name);
                linkingParentNode = null;
            }
        }
    }

    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
        DialogueNode foundNode = null;
        foreach (DialogueNode node in selectedDialogue.GetAllNodes()) 
        {
            if(node.GetRect().Contains(point)) 
            {
                foundNode= node;
            }
        }
        return foundNode;
    }
    
}

