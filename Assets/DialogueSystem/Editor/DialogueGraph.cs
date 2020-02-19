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
        private GraphView _graphView;
        private MiniMap _miniMap;
        private bool _miniMapEnabled = true;
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
            _toolbar = new Toolbar();
            _toolbar.styleSheets.Add(Resources.Load<StyleSheet>("Toolbar"));
            ToolbarButton addDialogueButton = new ToolbarButton(() =>
            {
                /* TODO: Wire up node creation */
            })
            {
                text = "Add Dialogue"
            };

            _toolbar.Add(addDialogueButton);
            _toolbar.Add(new ToolbarButton(() => { /* TODO: Wire up node creation */ })
            {
                text = "Add Condition"
            });
            
            _toolbar.Add(new ToolbarSpacer() {flex = true});
            
            TextField fileNameTextField = new TextField("FileName");
            fileNameTextField.style.minWidth = 150;
            fileNameTextField.labelElement.style.minWidth = 0;
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            _toolbar.Add(fileNameTextField);
            _toolbar.Add(new ToolbarButton(() => { /* TODO: Wire up Save operation */ })
            {
                text = "Save Asset"
            });
            _toolbar.Add(new ToolbarButton(() => { /* TODO: Wire up Load operation */ })
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