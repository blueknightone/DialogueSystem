using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    public class ConditionNode : BaseNode
    {
        public DialogueCondition ConditionToTest;
        private ObjectField _conditionObjectField;
        private Foldout _contentFoldout;
        private Toggle _conditionObjectToggle;
        private TextField _conditionNameTextField;
        private Button _createConditionAssetButton;
        private const string CONDITION_ASSET_PATH = "Assets/DialogueSystem/Conditions";

        public ConditionNode()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            BuildNodeControls();
            UpdateNodeFields();
        }

        public ConditionNode(ConditionNodeData nodeData)
        {
            GUID = nodeData.guid;
            ConditionToTest = nodeData.conditionToTest;

            styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            BuildNodeControls();
            UpdateNodeFields();
        }

        private void BuildNodeControls()
        {
            Port ifTruePort = GeneratePort(this, "If True", Direction.Output);
            ifTruePort.name = "ifTruePort";
            ifTruePort.portColor = Color.green;

            Port ifFalsePort = GeneratePort(this, "If False", Direction.Output);
            ifFalsePort.name = "ifFalsePort";
            ifFalsePort.portColor = Color.red;

            _contentFoldout = new Foldout {text = "Condition Properties", name = "condition-properties-foldout"};
            contentContainer.Add(_contentFoldout);

            _conditionObjectField = new ObjectField("Condition Object")
                {objectType = typeof(DialogueCondition), allowSceneObjects = false, value = ConditionToTest};

            _conditionObjectToggle = new Toggle("Initial Value") {bindingPath = "initialValue"};

            _conditionNameTextField = new TextField {value = "New Condition"};

            _createConditionAssetButton = new Button(() => CreateConditionAsset(_conditionNameTextField.value))
                {text = "New Condition"};
            _conditionObjectField.RegisterValueChangedCallback(evt =>
            {
                ConditionToTest = evt.newValue as DialogueCondition;
                _conditionObjectToggle.Unbind();
                
                if (ConditionToTest != null)
                {
                    SerializedObject so = new SerializedObject(ConditionToTest);
                    _conditionObjectToggle.Bind(so);
                }

                UpdateNodeFields();
            });
        }

        private void UpdateNodeFields()
        {
            title = ConditionToTest == null ? "Condition" : ConditionToTest.name;
            
            VisualElement contentFoldoutContentContainer = _contentFoldout.contentContainer;
            // Remove all children
            for (int i = 0; i < contentFoldoutContentContainer.childCount; i++)
            {
                contentFoldoutContentContainer.RemoveAt(i);
            }

            contentFoldoutContentContainer.Add(_conditionObjectField);

            if (ConditionToTest == null)
            {
                contentFoldoutContentContainer.Add(_conditionNameTextField);
                contentFoldoutContentContainer.Add(_createConditionAssetButton);
            }
            else
            {
                contentFoldoutContentContainer.Add(_conditionObjectToggle);
            }

            RefreshPorts();
            RefreshExpandedState();
        }

        private void CreateConditionAsset(string assetName)
        {
            DialogueCondition condition = CreateInstance<DialogueCondition>();
            if (!AssetDatabase.IsValidFolder(CONDITION_ASSET_PATH))
            {
                string[] folderNames = CONDITION_ASSET_PATH.Split('/');
                string parentFolder = string.Join("/", folderNames, 0, folderNames.Length - 1);
                AssetDatabase.CreateFolder(parentFolder, folderNames.Last());
            }

            AssetDatabase.CreateAsset(condition, $"{CONDITION_ASSET_PATH}/{assetName}.asset");
            _conditionNameTextField.value = "New Condition";
            _conditionObjectField.value = condition;
        }
    }
}