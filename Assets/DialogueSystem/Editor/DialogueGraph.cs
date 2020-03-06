using lastmilegames.DialogueSystem.DialogueGraphEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// Represents the parent EditorWindow for the dialogue visual editor.
    /// </summary>
    public class DialogueGraph : EditorWindow
    {
        /// <summary>
        /// The data utility for saving/loading graphs.
        /// </summary>
        private DialogueGraphDataUtility _dialogueGraphDataUtility;

        /// <summary>
        /// The filename to save/load the graph to.
        /// </summary>
        private string _fileName = "";
        
        /// <summary>
        /// The GraphView instance.
        /// </summary>
        private DialogueGraphView _graphView;

        /// <summary>
        /// The GraphView.MiniMap instance.
        /// </summary>
        private MiniMap _miniMap;

        /// <summary>
        /// Mini map visibility.
        /// </summary>
        private bool _miniMapEnabled;

        /// <summary>
        /// Opens the DialogueGraph editor window.
        /// </summary>
        [MenuItem("Dialogue System/Open Visual Editor")]
        private static void ShowWindow()
        {
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        private void OnEnable()
        {
            InitializeGraphView();
        }

        private void OnGUI()
        {
            // Enable mini map if visible.
            if (_miniMap != null) _miniMap.visible = _miniMapEnabled;
        }

        /// <summary>
        /// Prepares the window elements.
        /// </summary>
        private void InitializeGraphView()
        {
            // Set the root elements flex direction
            rootVisualElement.style.flexDirection = FlexDirection.Column;
            
            // Create the window elements
            rootVisualElement.Add(GenerateToolbar());
            
            _graphView = GenerateGraphView();
            rootVisualElement.Add(_graphView);
            
            _miniMap = GenerateMiniMap();
            _graphView.Add(_miniMap);

            // Get the data utility so we can save and load.
            _dialogueGraphDataUtility = new DialogueGraphDataUtility(_graphView);
        }

        /// <summary>
        /// Creates and populates a UIElements.Toolbar
        /// </summary>
        /// <returns>Returns the UIElements.Toolbar with controls.</returns>
        private Toolbar GenerateToolbar()
        {
            // Create the toolbar and load the stylesheet.
            Toolbar toolbar = new Toolbar();
            toolbar.styleSheets.Add(Resources.Load<StyleSheet>("Toolbar"));

            // Button to create new DialogueNodes.
            toolbar.Add(new ToolbarButton(() =>
            {
                _graphView.CreateNode(NodeType.Dialogue, position.size);
            }) {text = "Add Dialogue"});
            
            // Button to create new ConditionNodes
            toolbar.Add(new ToolbarButton(() =>
            {
                _graphView.CreateNode(NodeType.Condition, position.size);
            }) {text = "Add Condition"});

            // Flexible spacer.
            toolbar.Add(new ToolbarSpacer() {flex = true});

            // TODO: Save/Load asset from item selected in inspector. See OnOpenAsset
            // TextField to set the file to save/load from.
            TextField fileNameTextField = new TextField("FileName");
            fileNameTextField.style.minWidth = 150;
            fileNameTextField.labelElement.style.minWidth = 0;
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);
            
            // Button to save the asset
            toolbar.Add(new ToolbarButton(() =>
            {
                _dialogueGraphDataUtility.SaveGraph(fileNameTextField.value);
            })
            {
                text = "Save Asset"
            });
            
            // Button to load the asset
            toolbar.Add(new ToolbarButton(() =>
            {
                _dialogueGraphDataUtility.LoadGraph(fileNameTextField.value);
            })
            {
                text = "Load Asset"
            });

            // Button to toggle the mini map visibility.
            ToolbarToggle toggleMiniMap = new ToolbarToggle {text = "Toggle MiniMap"};
            toggleMiniMap.RegisterValueChangedCallback(evt => { _miniMapEnabled = evt.newValue; });
            toolbar.Add(toggleMiniMap);

            return toolbar;
        }

        /// <summary>
        /// Creates the default instance of DialogueGraphView.
        /// </summary>
        /// <returns>Returns a default DialogueGraphView.</returns>
        private static DialogueGraphView GenerateGraphView()
        {
            DialogueGraphView graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };
            graphView.style.flexGrow = 1;

            return graphView;
        }

        /// <summary>
        /// Creates a default mini map.
        /// </summary>
        /// <returns>Returns a default mini map to add to the DialogueGraphView.</returns>
        private MiniMap GenerateMiniMap()
        {
            MiniMap miniMap = new MiniMap {visible = _miniMapEnabled};
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            return miniMap;
        }
    }
}