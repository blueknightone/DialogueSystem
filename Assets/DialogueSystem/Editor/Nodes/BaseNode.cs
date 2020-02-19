using System;
using UnityEditor.Experimental.GraphView;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class BaseNode : Node
    {
        public string GUID { get; }

        public BaseNode()
        {
            GUID = Guid.NewGuid().ToString();
        }
    }
}