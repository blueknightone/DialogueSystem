using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.Nodes;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    public class ConversationEditorWindow : EditorWindow
    {
        private List<BaseNode> _nodes;
        private enum NodeType
        {
            DialogueNode,
            ResponseNode,
            ConditionNode
        }
        
        [MenuItem("Dialogue System/Visual Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<ConversationEditorWindow>();
            window.titleContent = new GUIContent("Conversation Visual Editor");
            window.Show();
        }

        private void OnGUI()
        {
            DrawNodes();
            
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if(GUI.changed) Repaint();
        }

        private void DrawNodes()
        {
            BeginWindows();
            if (_nodes != null)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].Draw(i);
                }
            }
            EndWindows();
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
            genericMenu.AddItem(new GUIContent("Add Dialogue Node"), false, () => OnClickContextItem(mousePosition, NodeType.DialogueNode));
            genericMenu.AddItem(new GUIContent("Add Response Node"), false, () => OnClickContextItem(mousePosition, NodeType.ResponseNode));
            genericMenu.AddItem(new GUIContent("Add Condition Node"), false, () => OnClickContextItem(mousePosition, NodeType.ConditionNode));
            genericMenu.ShowAsContext();
        }

        private void OnClickContextItem(Vector2 mousePosition, NodeType nodeType)
        {
            if (_nodes == null) _nodes = new List<BaseNode>();
            switch (nodeType)
            {
                case NodeType.DialogueNode:
                    DialogueNode dialogueNode = CreateInstance<DialogueNode>();
                    dialogueNode.nodeRect = new Rect(mousePosition.x, mousePosition.y, 200, 18);
                    _nodes.Add(dialogueNode);
                    break;
                case NodeType.ResponseNode:
                    ResponseNode responseNode = CreateInstance<ResponseNode>();
                    responseNode.nodeRect = new Rect(mousePosition.x, mousePosition.y, 100, 18);
                    _nodes.Add(responseNode);
                    break;
                case NodeType.ConditionNode:
                    ConditionNode conditionNode = CreateInstance<ConditionNode>();
                    conditionNode.nodeRect = new Rect(mousePosition.x, mousePosition.y, 100, 18);
                    _nodes.Add(conditionNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, "Invalid NodeType " + nameof(nodeType));
            }
        }
    }
}