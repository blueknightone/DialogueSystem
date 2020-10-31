using System;
using UnityEngine;

namespace lastmilegames.DialogueSystem.NodeData
{
    /// <summary>
    /// Base class for serializable node data. Represents the data all nodes should track.
    /// </summary>
    [Serializable]
    public class BaseNodeData
    {
        public string baseNodeGuid;
        public Vector2 position;
        public NodeType type;
    }
}