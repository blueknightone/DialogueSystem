using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CustomEditor(typeof(TriggerEventNode))]
    public class TriggerEventNodeEditor : NodeEditor
    {
        private SerializedObject sNode;
        private TriggerEventNode tNode;

        protected override VisualElement SetupEditorGUI()
        {
            tNode = (TriggerEventNode) node;
            sNode = new SerializedObject(tNode);

            var elementRoot = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/TriggerEventNodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/TriggerEventNodeEditor.uss");
            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            var evtTriggerEventList = elementRoot.Q<PropertyField>("evtTriggerEventList");
            elementRoot.Bind(sNode);

            return elementRoot;
        }
    }
}