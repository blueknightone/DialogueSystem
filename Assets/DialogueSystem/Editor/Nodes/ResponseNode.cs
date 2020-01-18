using System;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class ResponseNode : BaseNode
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] public string responseText = "";
        [SerializeField] public DialogueNode nextDialogueNode = null;
        [SerializeField] public ConditionNode conditionNode = null;
        // ReSharper restore RedundantDefaultMemberInitializer

        public ResponseNode(Vector2 position, float width, float height) : base(position, width, height)
        { }
        
        public override void Draw(int id)
        {
            GUILayout.Window(id, nodeRect, DrawNodeWindow, "Response");
        }

        private void DrawNodeWindow(int id)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Response Text");
            responseText = EditorGUILayout.TextField(responseText);
            
            EditorGUILayout.LabelField("Condition to Activate");
            conditionNode = (ConditionNode) EditorGUILayout.ObjectField(conditionNode, typeof(ConditionNode), false);
            
            EditorGUILayout.EndVertical();
        }

        protected override void ProcessContextMenu()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles actions when response is selected.
        /// </summary>
        /// <returns>Return the next dialogue node to display.</returns>
        public DialogueNode OnSelectResponse()
        {
            if (conditionNode != null) conditionNode.Activate();
            return nextDialogueNode;
        }
    }
}