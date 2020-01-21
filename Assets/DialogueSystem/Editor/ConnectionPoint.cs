using System;
using lastmilegames.DialogueSystem.Nodes;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    public enum ConnectionPointType
    {
        In,
        Out
    }

    public class ConnectionPoint
    {
        public Rect Rect;
        public readonly BaseNode Node;
        private readonly ConnectionPointType _pointType;
        private readonly GUIStyle _style;
        private readonly Action<ConnectionPoint> _onClickConnectionPoint;

        public ConnectionPoint(BaseNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
        {
            Node = node;
            _pointType = type;
            _style = style;
            _onClickConnectionPoint = onClickConnectionPoint;
            Rect = new Rect(0, 0, 10f, 20f);
        }

        public void Draw()
        {
            Rect.y = Node.Rect.y + Node.Rect.height * 0.5f - Rect.height * 0.5f;
            switch (_pointType)
            {
                case ConnectionPointType.In:
                    Rect.x = Node.Rect.x - Rect.width;
                    break;
                case ConnectionPointType.Out:
                    Rect.x = Node.Rect.x + Node.Rect.width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (GUI.Button(Rect, "", _style))
            {
                if (_onClickConnectionPoint != null)
                {
                    _onClickConnectionPoint(this);
                }
            }
        }
    }
}