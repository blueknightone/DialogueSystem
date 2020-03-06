using System;
using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// Represents the main GraphView for Dialogues
    /// </summary>
    public class DialogueGraphView : GraphView
    {
        /// <summary>
        /// Sets the initial settings for the DialogueGraphView instance.
        /// </summary>
        public DialogueGraphView()
        {
            // Load the stylesheet.
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

            // Setup features for zooming, dragging, and selecting.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Draw the grid.
            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // Create the start node.
            AddElement(GenerateEntryPointNode());
        }

        /// <summary>
        /// Get all ports compatible with a given port.
        /// </summary>
        /// <param name="startPort">Start port to validate against.</param>
        /// <param name="nodeAdapter">Node adapter</param>
        /// <returns>List of compatible ports.</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port // The target port is not itself
                    && startPort.node != port.node // The target port is not on the same node.
                    && startPort.direction != port.direction // The target port is not the same direction.
                )
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        /// <summary>
        /// Creates a node of the given type.
        /// </summary>
        /// <param name="nodeType">The type of node to create.</param>
        /// <param name="nodePosition">The position to create the node at.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided nodeType is invalid.</exception>
        public void CreateNode(NodeType nodeType, Vector2 nodePosition)
        {
            // TODO: Create node at middle of GraphView (GraphView.viewTransform.position?)
            switch (nodeType)
            {
                case NodeType.Condition:
                    AddElement(CreateConditionNode(nodePosition));
                    break;
                case NodeType.Dialogue:
                    AddElement(CreateDialogueNode(nodePosition));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
        }

        /// <summary>
        /// Creates a basic node to serve as an entry point.
        /// </summary>
        /// <returns>Returns a node with a single output port.</returns>
        private static EntryNode GenerateEntryPointNode()
        {
            EntryNode node = new EntryNode {title = "Start"};

            // Create the output port.
            node.GeneratePort(node, "Next", Direction.Output);
            
            // Prevent the deletion of the node.
            node.capabilities &= ~Capabilities.Deletable;
            
            // Set the default position of the node.
            node.SetPosition(new Rect(100, 200, 100, 150));
            
            return node;
        }

        /// <summary>
        /// Creates a new default ConditionNode at the given location.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>A condition node with default values.</returns>
        private static ConditionNode CreateConditionNode(Vector2 position)
        {
            ConditionNode node = new ConditionNode {title = "Condition"};
            node.SetPosition(new Rect(position, BaseNode.DefaultNodeSize));
            return node;
        }

        /// <summary>
        /// Creates a new Condition node with given values.
        /// </summary>
        /// <param name="nodeData">The values to initialize the node with.</param>
        /// <returns>A condition node with the initialized values.</returns>
        public static ConditionNode CreateConditionNode(ConditionNodeData nodeData)
        {
            return new ConditionNode(nodeData);
        }

        /// <summary>
        /// Creates a new DialogueNode with default values at a given position
        /// </summary>
        /// <param name="position">The position to create the node at.</param>
        /// <returns>A new dialogue node with default values.</returns>
        private DialogueNode CreateDialogueNode(Vector2 position)
        {
            DialogueNode node = new DialogueNode(OnClickRemoveOutputPort);
            node.SetPosition(new Rect(position, BaseNode.DefaultNodeSize));
            return node;
        }

        /// <summary>
        /// Creates a new DialogueNode with given values.
        /// </summary>
        /// <param name="nodeData">The values to initialize the node with.</param>
        /// <returns>A dialogue node initialized with the given values.</returns>
        public DialogueNode CreateDialogueNode(DialogueNodeData nodeData)
        {
            return new DialogueNode(OnClickRemoveOutputPort, nodeData);
        }

        /// <summary>
        /// Removes a port from a node and all of its connections.
        /// </summary>
        /// <param name="node">The port's parent node.</param>
        /// <param name="port">The port to remove.</param>
        private void OnClickRemoveOutputPort(Node node, Port port)
        {
            // Find all the edges (connections) this port makes to others.
            IEnumerable<Edge> targetEdges = edges.ToList()
                .Where(x => x.output == port && x.output.node == port.node)
                .ToList();

            // If any edges were found, remove them.
            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdges.First());
            }

            // If the node is a dialogueNode, we need to remove the ChoicePort element.
            if (node is DialogueNode dialogueNode)
            {
                List<ChoicePort> choicePorts = dialogueNode.ChoicePorts;
                choicePorts.Remove(choicePorts.Find(x => x.NodePort == port));
            }

            // Finally, remove the port and refresh the UI.
            node.outputContainer.Remove(port);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
    }
}