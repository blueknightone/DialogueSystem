using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    public enum NodeType
    {
        DialogueNode,
        ResponseNode,
        ConditionNode
    }

    public class ConversationEditorWindow : EditorWindow
    {
        private List<BaseNode> _nodes;
        private List<Connection> _connections;

        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _inPointStyle;
        private GUIStyle _outPointStyle;

        private ConnectionPoint _selectedInPoint;
        private ConnectionPoint _selectedOutPoint;

        [MenuItem("Dialogue System/Visual Editor")]
        private static void ShowWindow()
        {
            ConversationEditorWindow window = GetWindow<ConversationEditorWindow>();
            window.titleContent = new GUIContent("Conversation Visual Editor");
            window.Show();
        }

        private void OnEnable()
        {
            _nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("builtin skins/lightskin/images/node1.png") as Texture2D
                },
                border = new RectOffset(12, 12, 12, 12)
            };

            _selectedNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("builtin skins/lightskin/images/node1 on.png") as Texture2D
                },
                border = new RectOffset(12, 12, 12, 12)
            };

            _inPointStyle = new GUIStyle
            {
                normal =
                {
                    background =
                        EditorGUIUtility.Load("builtin skins/lightskin/images/btn left.png") as Texture2D
                },
                active =
                {
                    background =
                        EditorGUIUtility.Load("builtin skins/lightskin/images/btn left on.png") as Texture2D
                },
                border = new RectOffset(4, 4, 12, 12)
            };

            _outPointStyle = new GUIStyle
            {
                normal =
                {
                    background =
                        EditorGUIUtility.Load("builtin skins/lightskin/images/btn right.png") as Texture2D
                },
                active =
                {
                    background =
                        EditorGUIUtility.Load("builtin skins/lightskin/images/btn right on.png") as Texture2D
                },
                border = new RectOffset(4, 4, 12, 12)
            };
        }

        private void OnGUI()
        {
            DrawNodes();
            DrawConnections();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawNodes()
        {
            if (_nodes == null) return;
            BeginWindows();

            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Draw(i);
            }

            EndWindows();
        }

        private void DrawConnections()
        {
            if (_connections == null) return;
            foreach (Connection connection in _connections)
            {
                connection.Draw();
            }
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1) ProcessContextMenu(e.mousePosition);
                    break;
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            if (_nodes != null)
            {
                for (int i = _nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = _nodes[i].ProcessEvents(e);
                    if (guiChanged) GUI.changed = true;
                }
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Dialogue Node"), false,
                () => OnClickContextItem(mousePosition, NodeType.DialogueNode));
            genericMenu.AddItem(new GUIContent("Add Response Node"), false,
                () => OnClickContextItem(mousePosition, NodeType.ResponseNode));
            genericMenu.AddItem(new GUIContent("Add Condition Node"), false,
                () => OnClickContextItem(mousePosition, NodeType.ConditionNode));
            genericMenu.ShowAsContext();
        }

        private void OnClickContextItem(Vector2 mousePosition, NodeType nodeType)
        {
            if (_nodes == null) _nodes = new List<BaseNode>();
            switch (nodeType)
            {
                case NodeType.DialogueNode:
                    DialogueNode dialogueNode = new DialogueNode(
                        new Rect(mousePosition.x, mousePosition.y, 250, 50),
                        _nodeStyle, _selectedNodeStyle, _inPointStyle, _outPointStyle,
                        OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);

                    _nodes.Add(dialogueNode);
                    break;
                case NodeType.ResponseNode:
                    ResponseNode responseNode = new ResponseNode(
                        new Rect(mousePosition.x, mousePosition.y, 250, 50),
                        _nodeStyle, _selectedNodeStyle, _inPointStyle, _outPointStyle,
                        OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);

                    _nodes.Add(responseNode);
                    break;
                case NodeType.ConditionNode:
                    ConditionNode conditionNode = new ConditionNode(
                        new Rect(mousePosition.x, mousePosition.y, 250, 50),
                        _nodeStyle, _selectedNodeStyle, _inPointStyle, _outPointStyle,
                        OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);

                    _nodes.Add(conditionNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType,
                        "Invalid NodeType " + nameof(nodeType));
            }
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            _selectedInPoint = inPoint;
            if (_selectedOutPoint != null)
            {
                if (_selectedOutPoint.Node != _selectedInPoint.Node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            _selectedOutPoint = outPoint;
            if (_selectedInPoint != null)
            {
                if (_selectedOutPoint.Node != _selectedInPoint.Node)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            _connections.Remove(connection);
        }

        private void CreateConnection()
        {
            if (_connections == null)
            {
                _connections = new List<Connection>();
            }

            _connections.Add(new Connection(_selectedInPoint, _selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection()
        {
            _selectedInPoint = null;
            _selectedOutPoint = null;
        }

        private void OnClickRemoveNode(BaseNode node)
        {
            if (_connections != null)
            {
                List<Connection> connectionsToRemove = new List<Connection>();
                foreach (Connection connection in _connections)
                {
                    if (connection.InPoint == node.InPoint || connection.OutPoint == node.OutPoint)
                    {
                        connectionsToRemove.Add(connection);
                    }
                }

                for (int i = connectionsToRemove.Count - 1; i >= 0; i--)
                {
                    _connections.Remove(connectionsToRemove[i]);
                }
            }

            _nodes.Remove(node);
        }
    }
}