using System;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public abstract class BaseNode
    {
        public readonly ConnectionPoint InPoint;
        public readonly ConnectionPoint OutPoint;
        public Rect Rect;

        protected readonly string AssetDataPath;

        private readonly GUIStyle _selectedNodeStyle;
        private readonly GUIStyle _defaultNodeStyle;

        private readonly Action<BaseNode> OnRemoveNode;
        private GUIStyle _style;
        private bool _isSelected;
        private bool _isDragged;

        protected BaseNode(Rect rect,
            GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
            Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint,
            Action<BaseNode> onClickRemoveNode)
        {
            AssetDataPath = "Assets/DialogueSystem/Conversations/";
            Rect = rect;
            _style = nodeStyle;
            _defaultNodeStyle = nodeStyle;
            _selectedNodeStyle = selectedStyle;
            InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
            OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
            OnRemoveNode = onClickRemoveNode;
        }

        public void Drag(Vector2 delta)
        {
            Rect.position += delta;
        }

        public virtual void Draw(int id)
        {
            InPoint.Draw();
            OutPoint.Draw();
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            _isDragged = true;
                            GUI.changed = true;
                            _isSelected = true;
                            _style = _selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            _isSelected = false;
                            _style = _defaultNodeStyle;
                        }
                    }

                    if (e.button == 1 && Rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }

                    break;
                case EventType.MouseUp:
                    _isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && _isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Delete Node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickRemoveNode()
        {
            if (OnRemoveNode == null) return;
            OnRemoveNode(this);
        }
    }
}