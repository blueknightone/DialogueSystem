using System;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect nodeRect;
        public bool isDragged;

        protected string AssetDataPath;


        private void OnEnable()
        {
            AssetDataPath = "Assets/DialogueSystem/Conversations/";
        }

        protected BaseNode(Vector2 position, float width, float height)
        {
            nodeRect = new Rect(position.x, position.y, width, height);
        }

        private void Drag(Vector2 delta)
        {
            nodeRect.position += delta;
        }

        public abstract void Draw(int id);

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (nodeRect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                        }
                        else
                        {
                            GUI.changed = true;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        protected abstract void ProcessContextMenu();
    }
}