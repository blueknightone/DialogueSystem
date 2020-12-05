using UnityEditor;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public abstract class NodeEditor : Editor
    {
        private VisualElement root;

        private void OnEnable()
        {
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
            var baseNodeEditor = new VisualElement();
            baseNodeEditor.Add(new Label("baseNodeEditor"));

            return baseNodeEditor;
        }

        protected abstract VisualElement SetupEditorGUI();
    }
}