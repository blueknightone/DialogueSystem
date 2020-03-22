using lastmilegames.DialogueSystem.DialogueGraphEditor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    [CustomEditor(typeof(DialogueContainer))]
    public class DialogueContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DialogueContainer dialogueContainer = target as DialogueContainer;
            EditorGUILayout.BeginVertical();
            
            if (GUILayout.Button("Open Visual Editor"))
            {
                DialogueGraph.ShowWindow(dialogueContainer);
            }

            DrawDefaultInspector();
            EditorGUILayout.EndVertical();
        }
    }
}
