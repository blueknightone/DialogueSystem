using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public interface INode
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        NodeType Type { get; }

        /// <summary>
        /// The other nodes this node is connected to.
        /// </summary>
        List<string> Children { get; }

#if UNITY_EDITOR
        /// <summary>
        /// The position of the node.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Add a connection from this node to another.
        /// </summary>
        /// <param name="newChild">The <code>UnityEngine.Object.name</code> of the node to connect to.</param>
        void AddChild(string newChild);

        /// <summary>
        /// Remove a connection from this node to another.
        /// </summary>
        /// <param name="child">The <code>UnityEngine.Object.name</code> of the node to disconnect.</param>
        void RemoveChild(string child);

        /// <summary>
        /// Update the position of the node.
        /// </summary>
        /// <param name="newPosition"></param>
        void SetPosition(Vector2 newPosition);
#endif
    }
}