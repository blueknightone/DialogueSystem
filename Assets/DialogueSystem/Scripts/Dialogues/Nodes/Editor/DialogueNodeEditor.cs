using UnityEditor;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CustomEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : NodeEditor
    {
        protected override VisualElement SetupEditorGUI()
        {
            var childNodeEditor = new VisualElement();
            childNodeEditor.Add(new Label("DialogueNodeEditor"));

            return childNodeEditor;
        }
    }
}