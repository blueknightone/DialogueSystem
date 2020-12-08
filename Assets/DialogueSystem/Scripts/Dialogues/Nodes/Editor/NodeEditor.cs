using UnityEditor;
using UnityEditor.UIElements;
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
            var globalStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/_global.uss");
            root.styleSheets.Add(globalStyles);

            return root;
        }

        protected abstract VisualElement SetupEditorGUI();

        private VisualElement SetupBaseNodeEditor()
        {
            var elementRoot = new VisualElement();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uxml");
            visualTree.CloneTree(elementRoot);

            // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            //     "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uss");
            // elementRoot.styleSheets.Add(styleSheet);

            var txtDevNotes = elementRoot.Q<TextField>("txtDevNotes");
            txtDevNotes.value = node.DeveloperNotes;
            txtDevNotes.RegisterValueChangedCallback(evt => node.SetDevNote(evt.newValue));

            var v2PositionField = elementRoot.Q<Vector2Field>("v2Position");
            v2PositionField.value = node.Position;
            v2PositionField.RegisterValueChangedCallback(evt => node.SetPosition(evt.newValue));
            return elementRoot;
        }
    }
}