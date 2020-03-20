using System;

namespace lastmilegames.DialogueSystem
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
        /// The next node to move to.
        /// </summary>
        public string targetNodeGuid;
    }
}