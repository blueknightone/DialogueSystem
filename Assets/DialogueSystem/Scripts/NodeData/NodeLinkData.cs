using System;

namespace lastmilegames.DialogueSystem.NodeData
{
    /// <summary>
    /// Represents the links between nodes.
    /// </summary>
    [Serializable]
    public class NodeLinkData
    {
        /// <summary>
        /// The output node that starts the link.
        /// </summary>
        public string baseNodeGuid;

        /// <summary>
        /// The type of the output node that starts the link.
        /// </summary>
        public NodeType baseNodeType;

        /// <summary>
        /// The next node to move to.
        /// </summary>
        public string targetNodeGuid;

        /// <summary>
        /// The type of the final node in the link.
        /// </summary>
        public NodeType targetNodeType;
    }
}