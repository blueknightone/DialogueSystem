using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.ScriptableObject;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    /// <summary>
    /// Represents a conditional branch in the dialogue.
    /// </summary>
    public class ConditionNode : BaseNode
    {
        /// <summary>
        /// The UIElements.TextField for new condition names.
        /// </summary>
        private TextField _conditionNameTextField;
        
        /// <summary>
        /// The UIElements.ObjectField that selects the ConditionToTest. 
        /// </summary>
        private ObjectField _conditionObjectField;
        
        /// <summary>
        /// A UIElements.Toggle that binds and controls the initialValue of the _conditionObjectField's DialogueCondition.
        /// </summary>
        private Toggle _conditionObjectToggle;
        
        /// <summary>
        /// The UIElements.Foldout that contains all the controls.
        /// </summary>
        private Foldout _contentFoldout;
        
        /// <summary>
        /// The UIElements.Button for creating a new DialogueCondition asset.
        /// </summary>
        private Button _createConditionAssetButton;
        
        /// <summary>
        /// The DialogueCondition to test against and determine the next DialogueNode.
        /// </summary>
        public DialogueCondition ConditionToTest;

        /// <summary>
        /// Generates a new, default ConditionNode
        /// </summary>
        public ConditionNode()
        {
            type = NodeType.Condition;
            
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            GenerateOutputPorts();
            BuildNodeControls();
            UpdateNodeFields();
        }

        /// <summary>
        /// Generates a new ConditionNode with existing data.
        /// </summary>
        /// <param name="nodeData"></param>
        public ConditionNode(ConditionNodeData nodeData)
        {
            Guid = nodeData.guid;
            type = NodeType.Condition;
            ConditionToTest = nodeData.conditionToTest;

            styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            GenerateOutputPorts();
            BuildNodeControls();
            UpdateNodeFields();
        }

        /// <summary>
        /// Creates the true/false ports that branch the dialogue.
        /// </summary>
        private void GenerateOutputPorts()
        {
            Port ifTruePort = GeneratePort(this, "If True", Direction.Output);
            ifTruePort.name = "ifTruePort";
            ifTruePort.portColor = Color.green;

            Port ifFalsePort = GeneratePort(this, "If False", Direction.Output);
            ifFalsePort.name = "ifFalsePort";
            ifFalsePort.portColor = Color.red;
        }

        /// <summary>
        /// Builds the controls.
        /// </summary>
        private void BuildNodeControls()
        {
            // Create the containing foldout.
            _contentFoldout = new Foldout {text = "Condition Properties", name = "condition-properties-foldout"};
            contentContainer.Add(_contentFoldout);

            // Create the UIElements.Object field to select the DialogueCondition to test against.
            _conditionObjectField = new ObjectField("Condition Object")
                {objectType = typeof(DialogueCondition), allowSceneObjects = false, value = ConditionToTest};

            // Create the UIElements.Toggle that controls ConditionToTest.initialValue
            _conditionObjectToggle = new Toggle("Initial Value") {bindingPath = "initialValue"};
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

            // Create the UIElements.TextField that names a new DialogueCondition asset.
            _conditionNameTextField = new TextField
            {
                name = "conditionNameTextField",
                value = "New Condition"
            };

            // Create the UIElements.Button that will create a new DialogueCondition asset with the name in _conditionNameTextField 
            _createConditionAssetButton = new Button(() => CreateConditionAsset(_conditionNameTextField.value))
                {text = "New Condition"};
        }

        /// <summary>
        /// Updates the form state based on a selected ConditionToTest.
        /// </summary>
        private void UpdateNodeFields()
        {
            VisualElement contentFoldoutContentContainer = _contentFoldout.contentContainer;
            
            // Update the title
            title = ConditionToTest == null ? "Condition" : ConditionToTest.name;
            
            // Remove all children from the foldout in preparation for new state.
            for (int i = contentFoldoutContentContainer.childCount - 1; i >= 0; i--)
            {
                contentFoldoutContentContainer.RemoveAt(i);
            }

            // Add the object field
            contentFoldoutContentContainer.Add(_conditionObjectField);

            // If there is no condition object set...
            if (ConditionToTest == null)
            {
                // ... add the fields to create one, or...
                contentFoldoutContentContainer.Add(_conditionNameTextField);
                contentFoldoutContentContainer.Add(_createConditionAssetButton);
            }
            else
            {
                // ...add the field to toggle the condition's initial value.
                contentFoldoutContentContainer.Add(_conditionObjectToggle);
            }

            // Refresh the node.
            RefreshPorts();
            RefreshExpandedState();
        }

        /// <summary>
        /// Creates a new DialogueCondition in the Assets folder.
        /// </summary>
        /// <param name="assetName">The file name for the new asset.</param>
        private void CreateConditionAsset(string assetName)
        {
            string assetPath = $"Assets/{assetName}.asset";
            bool writeFile = true;
            
            // Check if the file already exists.
            if (File.Exists(assetPath))
            {
                writeFile = EditorUtility.DisplayDialog(
                    "Existing File Found",
                    "A file with the same name already exists.Overwrite?",
                    "Overwrite",
                    "Cancel"
                );
            }

            // If it isn't okay to write the file, exit early.
            if (!writeFile) return;
            
            // Create the DialogueCondition asset.
            DialogueCondition condition = CreateInstance<DialogueCondition>();
            // Save the asset file.
            AssetDatabase.CreateAsset(condition, assetPath);
            
            // Reset the text field and select the new DialogueCondition.
            _conditionNameTextField.value = "New Condition";
            _conditionObjectField.value = condition;
        }
    }
}