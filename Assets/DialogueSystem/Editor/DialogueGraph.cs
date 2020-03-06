using lastmilegames.DialogueSystem.DialogueGraphEditor;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    public class DialogueGraph : EditorWindow
    {
        private Toolbar _toolbar;
        private DialogueGraphView _graphView;
        private MiniMap _miniMap;
        private DialogueGraphDataUtility _dialogueGraphDataUtility;
        private bool _miniMapEnabled;
        private string _fileName = "";

        [MenuItem("Dialogue System/Open Visual Editor")]
        private static void ShowWindow()
        {
            DialogueGraph window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexDirection = FlexDirection.Column;

            GenerateToolbar();
            GenerateGraphView();
            GenerateMiniMap();
            
            _dialogueGraphDataUtility = new DialogueGraphDataUtility(_graphView);
        }

        private void OnGUI()
        {
            if (_miniMap != null)
            {
                _miniMap.visible = _miniMapEnabled;
            }
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_toolbar);
        }

        private void GenerateToolbar()
        {
            // TODO: Load asset from item selected in inspector
            _toolbar = new Toolbar();
            _toolbar.styleSheets.Add(Resources.Load<StyleSheet>("Toolbar"));

            _toolbar.Add(new ToolbarButton(() =>
            {
                _graphView.CreateNode("Dialogue", NodeType.Dialogue, position.size);
            }) {text = "Add Dialogue"});
            
            _toolbar.Add(new ToolbarButton(() =>
            {
                _graphView.CreateNode("Condition", NodeType.Condition, position.size);
            }) {text = "Add Condition"});

            _toolbar.Add(new ToolbarSpacer() {flex = true});

            TextField fileNameTextField = new TextField("FileName");
            fileNameTextField.style.minWidth = 150;
            fileNameTextField.labelElement.style.minWidth = 0;
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            _toolbar.Add(fileNameTextField);
            _toolbar.Add(new ToolbarButton(() =>
            {
                _dialogueGraphDataUtility.SaveGraph(fileNameTextField.value);
            })
            {
                text = "Save Asset"
            });
            _toolbar.Add(new ToolbarButton(() =>
            {
                _dialogueGraphDataUtility.LoadGraph(fileNameTextField.value);
            })
            {
                text = "Load Asset"
            });

            ToolbarToggle toggleMiniMap = new ToolbarToggle {text = "Toggle MiniMap"};
            toggleMiniMap.RegisterValueChangedCallback(evt => { _miniMapEnabled = evt.newValue; });
            _toolbar.Add(toggleMiniMap);

            rootVisualElement.Add(_toolbar);
        }

        private void GenerateGraphView()
        {
            _graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };
            _graphView.style.flexGrow = 1;
            rootVisualElement.Add(_graphView);
        }

        private void GenerateMiniMap()
        {
            _miniMap = new MiniMap {visible = _miniMapEnabled};
            _miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(_miniMap);
        }
    }
}