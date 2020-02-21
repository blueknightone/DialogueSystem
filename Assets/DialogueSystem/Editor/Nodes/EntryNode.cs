using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class EntryNode : BaseNode
    {
        public EntryNode()
        {
            topContainer.Remove(inputContainer);
        }
    }
}