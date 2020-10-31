using System;
using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// Represents the main GraphView for Dialogues
    /// </summary>
    public class DialogueGraphView : GraphView
    {
        private SearchWindowProvider _searchWindow;
        /// <summary>
        /// Sets the initial settings for the DialogueGraphView instance.
        /// </summary>
        public DialogueGraphView(EditorWindow editorWindow)
        {
            // Load the stylesheet.
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

            // Setup features for zooming, dragging, and selecting.
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Draw the grid.
            // GridBackground grid = new GridBackground();
            // Insert(0, grid);
            // grid.StretchToParentSize();

            // Create the start node.
            AddElement(GenerateEntryPointNode());

            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(EditorWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindow.Init(editorWindow, this);
            nodeCreationRequest = context => 
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
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
        /// Creates a basic node to serve as an entry point.
        /// </summary>
        /// <returns>Returns a node with a single output port.</returns>
        private static EntryNode GenerateEntryPointNode()
        {
            EntryNode node = new EntryNode {title = "Start"};
            
            // Remove the entry port
            node.inputContainer.RemoveAt(0);

            // Create the output port.
            node.GeneratePort(node, "Next", Direction.Output);

            // Prevent the deletion of the node.
            node.capabilities &= ~Capabilities.Deletable;

            // Set the default position of the node.
            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        /// <summary>
        /// Creates a node of the given type.
        /// </summary>
        /// <param name="nodeType">The type of node to create.</param>
        /// <param name="position">The position to create the node at.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided nodeType is invalid.</exception>
        public void CreateNode(NodeType nodeType, Vector2 position)
        {
            switch (nodeType)
            {
                case NodeType.Condition:
                    AddElement(CreateConditionNode(position));
                    break;
                case NodeType.Dialogue:
                    AddElement(CreateDialogueNode(position));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
        }

        /// <summary>
        /// Creates a new default ConditionNode at the given location.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>A condition node with default values.</returns>
        private ConditionNode CreateConditionNode(Vector2 position)
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
        public ConditionNode CreateConditionNode(ConditionNodeData nodeData)
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