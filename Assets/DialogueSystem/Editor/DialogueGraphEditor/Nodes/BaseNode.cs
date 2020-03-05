using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    /// <summary>
    /// The base class all DialogueGraphEditor nodes inherit from.
    /// </summary>
    public abstract class BaseNode : Node
    {
        /// <summary>
        /// The default minimum size of a node.
        /// Nodes will expand based on USS rules.
        /// </summary>
        public static readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

        /// <summary>
        /// The node's unique identifier. 
        /// </summary>
        public string GUID { get; protected set; }

        /// <summary>
        /// The action delegate to call when the remove port button is clicked.
        /// </summary>
        protected readonly Action<Node, Port> OnClickRemovePort;

        /// <summary>
        /// Creates a default node with an input port.
        /// </summary>
        protected BaseNode()
        {
            GUID = Guid.NewGuid().ToString();

            GeneratePort(this, "Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Q<Port>().portColor = Color.cyan;
            OnClickRemovePort = (node, port) =>
            {
                Debug.LogWarningFormat(
                    "No remove port action set on node {0} port {1}",
                    port.parent.IndexOf(port),
                    GUID
                );
            };
        }

        /// <summary>
        /// Generates a node with an onClickRemovePort event.
        /// </summary>
        /// <param name="onClickRemovePort">Event that fires when remove port button is clicked.</param>
        protected BaseNode(Action<Node, Port> onClickRemovePort)
        {
            GUID = Guid.NewGuid().ToString();
            OnClickRemovePort = onClickRemovePort;

            // Create input port
            GeneratePort(this, "Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Q<Port>().portColor = Color.cyan;
        }

        /// <summary>
        /// Generates a new standard GraphView.Port
        /// </summary>
        /// <param name="node">Node to add port to.</param>
        /// <param name="portName">Name of the port.</param>
        /// <param name="portDirection">Whether the port is an input or output port.</param>
        /// <param name="capacity">Whether the port can only accept one connection or multiple.</param>
        /// <param name="type">The value type of the port.</param>
        /// <returns>Return the generated port,</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid direction is provided.</exception>
        public Port GeneratePort(Node node, string portName, Direction portDirection, Port.Capacity capacity
            = Port.Capacity.Single, Type type = null)
        {
            Port port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, type);
            port.portName = portName;

            switch (portDirection)
            {
                case Direction.Input:
                    node.inputContainer.Add(port);
                    break;
                case Direction.Output:
                    node.outputContainer.Add(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portDirection), portDirection, null);
            }

            return port;
        }
    }
}