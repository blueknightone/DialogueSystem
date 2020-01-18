using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class ConditionNode : BaseNode
    {
        private bool _activated;
        public bool Activated => _activated;

        public ConditionNode(Vector2 position, float width, float height) : base(position, width, height)
        { }

        public override void Draw(int id)
        {
            GUILayout.Window(id, nodeRect, DrawNodeWindow, "Condition");
        }

        private void DrawNodeWindow(int windowId)
        {
            EditorGUILayout.BeginVertical();
            
            _activated = EditorGUILayout.Toggle("Activated", _activated);
            
            EditorGUILayout.EndVertical();
        }

        public void Activate()
        {
            _activated = true;
        }

        protected override void ProcessContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}