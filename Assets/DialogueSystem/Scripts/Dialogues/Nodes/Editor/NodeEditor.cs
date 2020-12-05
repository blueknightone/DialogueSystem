using UnityEditor;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public abstract class NodeEditor : Editor
    {
        protected Node node;
        private VisualElement root;

        private void OnEnable()
        {
            node = (Node) target;
            root = new VisualElement();

            root.Add(SetupEditorGUI());
            root.Add(SetupBaseNodeEditor());
        }

        public override VisualElement CreateInspectorGUI()
        {
            return root;
        }

        private VisualElement SetupBaseNodeEditor()
        {
            var elementRoot = new VisualElement();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uss");

            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            return elementRoot;
        }

        protected abstract VisualElement SetupEditorGUI();
    }
}