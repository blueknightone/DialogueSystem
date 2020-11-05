using System.Linq;
using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// Represents the parent EditorWindow for the dialogue visual editor.
    /// </summary>
    public class DialogueGraph : EditorWindow
    {
        /// <summary>
        /// The data utility for saving/loading graphs.
        /// </summary>
        private static DialogueGraphDataUtility dialogueGraphDataUtility;

        private static DialogueContainer dialogueContainer;

        /// <summary>
        /// The GraphView instance.
        /// </summary>
        private DialogueGraphView _graphView;

        /// <summary>
        /// The GraphView.MiniMap instance.
        /// </summary>
        private MiniMap _miniMap;

        private BlackboardProvider blackboardProvider;

        /// <summary>
        /// Mini map visibility.
        /// </summary>
        private bool _miniMapEnabled;

        /// <summary>
        /// Opens the DialogueGraph editor window.
        /// </summary>
        public static void ShowWindow(DialogueContainer container)
        {
            dialogueContainer = container;

            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent(dialogueContainer.name);
            window.Show();
        }

        [MenuItem("Dialogue Editor/Open Editor Window")]
        [MenuItem("Assets/DialogueEditor/Open Editor Window")]
        public static bool ShowEditor()
        {
            dialogueContainer = Selection.activeObject as DialogueContainer;

            if (dialogueContainer)
            {
                ShowWindow(dialogueContainer);
                return true;
            }

            return false;
        }

        [MenuItem("Dialogue Editor/Open Editor Window", true)]
        [MenuItem("Assets/DialogueEditor/Open Editor Window", true)]
        public static bool ValidateValidObjectSelected()
        {
            return Selection.activeObject is DialogueContainer;
        }

        [MenuItem("Dialogue Editor/New Dialogue Container")]
        public static void NewDialogue()
        {
            var container = CreateInstance<DialogueContainer>();
            string path = EditorUtility.SaveFilePanel(
                "New Dialogue Container",
                "Assets/",
                "New Dialogue Container.asset",
                "asset"
            );

            if (path.Length == 0) return;

            if (dialogueGraphDataUtility == null)
            {
                dialogueGraphDataUtility = new DialogueGraphDataUtility();
            }

            dialogueGraphDataUtility.SaveGraph(path, container);

            bool openDialogueEditor = EditorUtility.DisplayDialog(
                "Open Dialogue Editor Now?",
                "Would you like to open the Dialogue Editor and edit this conversation now?",
                "Yes",
                "No"
            );

            if (openDialogueEditor)
            {
                ShowWindow(container);
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceId);
            var container = AssetDatabase.LoadAssetAtPath<DialogueContainer>(assetPath);

            if (container)
            {
                ShowWindow(container);
                return true;
            }

            return false;
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

            blackboardProvider = new BlackboardProvider(_graphView);
            _graphView.Add(blackboardProvider.Blackboard);

            // Get the data utility so we can save and load.
            dialogueGraphDataUtility = new DialogueGraphDataUtility(_graphView);

            if (dialogueContainer != null) dialogueGraphDataUtility.LoadGraph(dialogueContainer, blackboardProvider);
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

            // Button to save the asset
            toolbar.Add(new ToolbarButton(() => { dialogueGraphDataUtility.SaveGraph(dialogueContainer); })
            {
                text = "Save Asset"
            });

            // Flexible spacer.
            toolbar.Add(new ToolbarSpacer() {flex = true});

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
        private DialogueGraphView GenerateGraphView()
        {
            DialogueGraphView graphView = new DialogueGraphView(this)
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
            var miniMap = new MiniMap {visible = _miniMapEnabled};
            Vector2 coords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
            return miniMap;
        }
    }
}