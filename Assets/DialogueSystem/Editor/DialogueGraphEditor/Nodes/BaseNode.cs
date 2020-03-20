using System;
using System.Collections.Generic;
using System.Linq;
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
        protected readonly Action<Node, Port> onClickRemovePort;

        /// <summary>
        /// Creates a default node with an input port.
        /// </summary>
        protected BaseNode()
        {
            GUID = Guid.NewGuid().ToString();

            GeneratePort(this, "Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Q<Port>().portColor = Color.cyan;
            onClickRemovePort = (node, port) =>
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
            this.onClickRemovePort = onClickRemovePort;

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

    public static class BaseNodeExtension
    {
        /// <summary>
        /// Removes a port from a node and all of its connections.
        /// </summary>
        /// <param name="node">The port's parent node.</param>
        /// <param name="port">The port to remove.</param>
        public static void OnClickRemovePort(this BaseNode node, Port port, DialogueGraphView targetGraph)
        {
            // Find all the edges (connections) this port makes to others.
            IEnumerable<Edge> targetEdges = targetGraph.edges.ToList()
                .Where(x => x.output == port && x.output.node == port.node)
                .ToList();

            // If any edges were found, remove them.
            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();
                edge.input.Disconnect(edge);
                targetGraph.RemoveElement(targetEdges.First());
            }

            // If the node is a dialogueNode, we need to remove the DialogueNodePort element.
            if (node is DialogueNode dialogueNode)
            {
                List<DialogueNodePort> choicePorts = dialogueNode.DialogueNodePorts;
                choicePorts.Remove(choicePorts.Find(x => x.Port == port));
            }

            // Finally, remove the port and refresh the UI.
            node.outputContainer.Remove(port);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
    }
}