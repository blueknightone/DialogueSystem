using lastmilegames.DialogueSystem.NodeData;
using UnityEngine;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    /// <summary>
    /// Represents the empty starting point for each Dialogue.
    /// </summary>
    public class EntryNode : BaseNode
    {
        public EntryNode()
        {
            type = NodeType.Entry;
            topContainer.Remove(inputContainer);
        }
    }
}