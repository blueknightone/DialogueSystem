using UnityEditor;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CustomEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : NodeEditor
    {
        private DialogueNode dialogueNode;

        protected override VisualElement SetupEditorGUI()
        {
            dialogueNode = node as DialogueNode;
            var elementRoot = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/DialogueNodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/DialogueNodeEditor.uss");

            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            return elementRoot;
        }
    }
}