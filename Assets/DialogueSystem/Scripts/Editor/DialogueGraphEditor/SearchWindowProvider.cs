using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView _graphView;
        private EditorWindow _editorWindow;
        private Texture2D _indentationIcon; // Icon for indentation hack 

        public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
        {
            _graphView = graphView;
            _editorWindow = editorWindow;

            // Transparent icon to fix indentation in search window groups
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Content"), 1),
                new SearchTreeEntry(new GUIContent("Dialogue", _indentationIcon))
                {
                    level = 2,
                    userData = NodeType.Dialogue
                },
                new SearchTreeEntry(new GUIContent("Condition", _indentationIcon))
                {
                    level = 2,
                    userData = NodeType.Condition
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(
                _editorWindow.rootVisualElement.parent,
                context.screenMousePosition - _editorWindow.position.position
            );

            Vector2 localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

            switch (searchTreeEntry.userData)
            {
                case NodeType.Dialogue:
                    _graphView.CreateNode(NodeType.Dialogue, localMousePosition);
                    return true;
                case NodeType.Condition:
                    _graphView.CreateNode(NodeType.Condition, localMousePosition);
                    return true;
                default:
                    return false;
            }
        }
    }
}