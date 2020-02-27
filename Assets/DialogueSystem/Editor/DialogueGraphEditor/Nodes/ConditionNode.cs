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
        private const string CONDITION_ASSET_PATH = "Assets/DialogueSystem/Conditions";

        public ConditionNode()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            BuildNodeControls();
        }
        
        private void BuildNodeControls()
        {
            Port ifTruePort = GeneratePort(this, "If True", Direction.Output);
            ifTruePort.portColor = Color.green;

            Port ifFalsePort = GeneratePort(this, "If False", Direction.Output);
            ifFalsePort.portColor = Color.red;
            
            Foldout contentFoldout = new Foldout
            {
                text = "Condition Properties",
                name = "condition-properties-foldout"
            };
            contentContainer.Add(contentFoldout);
            VisualElement contentFoldoutContentContainer = contentFoldout.contentContainer;

            _conditionObjectField = new ObjectField("Condition Object")
            {
                objectType = typeof(DialogueCondition),
                allowSceneObjects = false
            };

            Toggle conditionObjectToggle = new Toggle("Initial Value")
            {
                bindingPath = "initialValue"
            };
            
            TextField newConditionName = new TextField
            {
                value = "New Condition"
            };

            Button newConditionAssetButton = new Button(() => CreateConditionAsset(newConditionName.value))
            {
                text = "New Condition"
            };
            
            _conditionObjectField.RegisterValueChangedCallback(evt =>
            {
                DialogueCondition dc = evt.newValue as DialogueCondition;
                ConditionToTest = dc;
                if (dc != null)
                {
                    title = dc.name;
                    
                    SerializedObject so = new SerializedObject(dc);
                    conditionObjectToggle.Bind(so);

                    if (!contentFoldoutContentContainer.Contains(conditionObjectToggle))
                    {
                        contentFoldoutContentContainer.Add(conditionObjectToggle);
                    }

                    if (contentFoldoutContentContainer.Contains(newConditionName))
                    {
                        contentFoldoutContentContainer.Remove(newConditionName);
                    }

                    if (contentFoldoutContentContainer.Contains(newConditionAssetButton))
                    {
                        contentFoldoutContentContainer.Remove(newConditionAssetButton);
                    }
                }
                else
                {
                    title = "Condition";
                    conditionObjectToggle.Unbind();
                    if (contentFoldoutContentContainer.Contains(conditionObjectToggle))
                    {
                        contentFoldoutContentContainer.Remove(conditionObjectToggle);
                    }

                    if (!contentFoldoutContentContainer.Contains(newConditionName))
                    {
                        contentFoldoutContentContainer.Add(newConditionName);
                    }

                    if (!contentFoldoutContentContainer.Contains(newConditionAssetButton))
                    {
                        contentFoldoutContentContainer.Add(newConditionAssetButton);
                    }
                }

                RefreshExpandedState();
            });


            contentFoldoutContentContainer.Add(_conditionObjectField);
            contentFoldoutContentContainer.Add(newConditionName);
            contentFoldoutContentContainer.Add(newConditionAssetButton);
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
            _conditionObjectField.value = condition;
        }
    }
}