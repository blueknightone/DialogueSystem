using System;
using System.Collections.Generic;

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
        /// The response text associated with the link.
        /// </summary>
        public string choiceText;
        
        /// <summary>
        /// The DialogueCondition to have it's value toggled.
        /// </summary>
        public DialogueCondition dialogueConditionToToggle;
        
        /// <summary>
        /// The next node to move to.
        /// </summary>
        public List<string> targetNodeGuid;
    }
}