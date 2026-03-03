using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    [MenuItem("Window/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
        //utility parameters: type of window, utility, title of window
        GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");

    }
}

