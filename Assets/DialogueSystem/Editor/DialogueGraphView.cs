using System;
using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

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
            switch (nodeType)
            {
                case NodeType.Condition:
                    AddElement(CreateConditionNode(nodeName, windowSize));
                    break;
                case NodeType.Dialogue:
                    AddElement(CreateDialogueNode(nodeName, windowSize));
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

        private GraphElement CreateConditionNode(string nodeName, Vector2 windowSize)
        {
            ConditionNode node = new ConditionNode {title = nodeName};
            node.SetPosition(new Rect(windowSize / 2f, node.DefaultNodeSize));
            return node;
        }

        private DialogueNode CreateDialogueNode(string nodeName, Vector2 windowSize)
        {
            DialogueNode node = new DialogueNode(OnClickRemoveOutputPort)
            {
                title = nodeName,
                DialogueText = nodeName,
            };
            node.SetPosition(new Rect(windowSize / 2f, node.DefaultNodeSize));

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

            node.outputContainer.Remove(port);
        }
    }
}