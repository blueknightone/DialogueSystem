using System;
using System.Collections.Generic;

namespace lastmilegames.DialogueSystem.NodeData
{
    /// <summary>
    /// Represents the data a DialogueNode should track.
    /// </summary>
    [Serializable]
    public class DialogueNodeData : BaseNodeData
    {
        /// <summary>
        /// The spoken dialogue.
        /// </summary>
        public string dialogueText;
        
        /// <summary>
        /// The speaker.
        /// </summary>
        public string speakerName;

        /// <summary>
        /// A list of the DialogueNodePort responses.
        /// </summary>
        public List<string> responses;

        /// <summary>
        /// A list of the DialogueNodePort conditions to toggle.
        /// </summary>
        public List<DialogueCondition> conditionsToToggle;
    }
}