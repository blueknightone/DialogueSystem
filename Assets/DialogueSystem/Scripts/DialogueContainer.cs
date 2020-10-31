using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// Represents a conversation dialogue.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueContainer", menuName = "Dialogue System/New Container", order = 0)]
    public class DialogueContainer : ScriptableObject
    {
        /// <summary>
        /// The entry node for the conversation.
        /// </summary>
        public BaseNodeData entryNode = new BaseNodeData
        {
            baseNodeGuid = Guid.NewGuid().ToString(),
            position = new Vector2(100, 200),
            type = NodeType.Entry
        };

        /// <summary>
        /// A list of the links between nodes.
        /// </summary>
        public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
        
        /// <summary>
        /// A list of dialogues which contain who is speaking, what they're saying, and available responses.
        /// </summary>
        public List<DialogueNodeData> dialogueNodeData = new List<DialogueNodeData>();
        
        /// <summary>
        /// A list of conditions that can alter the flow of the conversation.
        /// </summary>
        public List<ConditionNodeData> conditionNodeData = new List<ConditionNodeData>();
    }
}