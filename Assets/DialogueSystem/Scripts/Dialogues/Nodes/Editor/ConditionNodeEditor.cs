using lastmilegames.DialogueSystem.Conditions;
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
            ToggleField(boolConditionValueField, cNode.Condition != null);

            var objConditionField = elementRoot.Q<ObjectField>("objConditionField");
            objConditionField.objectType = typeof(Condition);
            objConditionField.value = cNode.Condition;
            objConditionField.RegisterValueChangedCallback(evt =>
            {
                cNode.SetCondition((Condition) evt.newValue);
                ToggleField(boolConditionValueField, cNode.Condition != null);
            });

            return elementRoot;
        }

        private void ToggleField(VisualElement field, bool condition)
        {
            field.style.display = condition ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}