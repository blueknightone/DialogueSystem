using lastmilegames.DialogueSystem.Conditions;
using lastmilegames.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CustomEditor(typeof(ConditionNode))]
    public class ConditionNodeEditor : NodeEditor
    {
        private ConditionNode cNode;

        protected override VisualElement SetupEditorGUI()
        {
            cNode = (ConditionNode) node;
            var elementRoot = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/ConditionNodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/ConditionNodeEditor.uss");
            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            var boolConditionValueField = elementRoot.Q<Toggle>();
            boolConditionValueField.value = cNode.Condition && cNode.Condition.InitialValue;
            boolConditionValueField.RegisterValueChangedCallback(evt =>
            {
                cNode.Condition.SetValue(evt.newValue);
                boolConditionValueField.ToggleFieldVisibility(cNode.Condition != null);
            });
            boolConditionValueField.ToggleFieldVisibility(cNode.Condition != null);

            var enumNodeFunctionField = elementRoot.Q<EnumField>("enumNodeFunctionField");
            enumNodeFunctionField.value = cNode.NodeFunction;
            enumNodeFunctionField.RegisterValueChangedCallback(evt =>
                cNode.SetFunction((NodeFunction) evt.newValue));

            var objConditionField = elementRoot.Q<ObjectField>("objConditionField");
            objConditionField.objectType = typeof(Condition);
            objConditionField.value = cNode.Condition;
            objConditionField.RegisterValueChangedCallback(evt =>
            {
                cNode.SetCondition((Condition) evt.newValue);
                boolConditionValueField.ToggleFieldVisibility(cNode.Condition != null);
            });

            return elementRoot;
        }
    }
}