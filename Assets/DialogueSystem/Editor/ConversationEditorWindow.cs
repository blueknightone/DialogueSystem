using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.Nodes;
using UnityEditor;
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

        private Vector2 _offset;
        private Vector2 _drag;

        [MenuItem("Window/Conversation Editor")]
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
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            _offset += _drag * 0.5f;
            Vector3 newOffset = new Vector3(_offset.x % gridSpacing, _offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                    new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                    new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
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

        private void DrawConnectionLine(Event e)
        {
            if (_selectedInPoint != null && _selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    _selectedInPoint.Rect.center,
                    e.mousePosition,
                    _selectedInPoint.Rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (_selectedOutPoint != null && _selectedInPoint == null)
            {
                Handles.DrawBezier(
                    _selectedOutPoint.Rect.center,
                    e.mousePosition,
                    _selectedOutPoint.Rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );
                GUI.changed = true;
            }
        }

        private void ProcessEvents(Event e)
        {
            _drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1) ProcessContextMenu(e.mousePosition);
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }

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

        private void OnDrag(Vector2 delta)
        {
            if (_nodes == null) return;

            _drag = delta;

            foreach (BaseNode node in _nodes)
            {
                node.Drag(delta);
            }

            GUI.changed = true;
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