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
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && (startPort.direction != port.direction))
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void CreateNode(string nodeName, NodeType nodeType, Vector2 windowSize)
        {
            // TODO: Create node at middle of GraphView (GraphView.viewTransform.position?)
            switch (nodeType)
            {
                case NodeType.Condition:
                    AddElement(CreateConditionNode(windowSize));
                    break;
                case NodeType.Dialogue:
                    AddElement(CreateDialogueNode(windowSize));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
        }

        private EntryNode GenerateEntryPointNode()
        {
            EntryNode node = new EntryNode
            {
                title = "Start"
            };
            node.GeneratePort(node,"Next", Direction.Output);
            node.capabilities &= ~Capabilities.Deletable;
            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public ConditionNode CreateConditionNode(Vector2 windowSize)
        {
            ConditionNode node = new ConditionNode {title = "Condition"};
            node.SetPosition(new Rect(windowSize / 2f, BaseNode.DefaultNodeSize));
            return node;
        }

        public ConditionNode CreateConditionNode(ConditionNodeData nodeData)
        {
            ConditionNode node = new ConditionNode(nodeData);
            return node;
        }

        public DialogueNode CreateDialogueNode(Vector2 windowSize)
        {
            DialogueNode node = new DialogueNode(OnClickRemoveOutputPort);
            node.UpdateTitle();
            node.SetPosition(new Rect(windowSize / 2f, BaseNode.DefaultNodeSize));

            return node;
        }

        public DialogueNode CreateDialogueNode(Vector2 windowSize, DialogueNodeData nodeData)
        {
            DialogueNode node = new DialogueNode(OnClickRemoveOutputPort, nodeData);
            node.UpdateTitle();
            node.SetPosition(new Rect(windowSize / 2f, BaseNode.DefaultNodeSize));

            return node;
        }

        private void OnClickRemoveOutputPort(Node node, Port port)
        {
            IEnumerable<Edge> allEdges = edges.ToList();
            IEnumerable<Edge> targetEdges = allEdges.Where(x =>
                x.output == port && x.output.node == port.node
            ).ToList();

            if (targetEdges.Any())
            {
                Edge edge = targetEdges.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdges.First());
            }

            if (node is DialogueNode dialogueNode)
            {
                List<ChoicePort> choicePorts = dialogueNode.ChoicePorts;
                choicePorts.Remove(choicePorts.Find(x => x.NodePort == port));
            }

            node.outputContainer.Remove(port);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }
    }
}