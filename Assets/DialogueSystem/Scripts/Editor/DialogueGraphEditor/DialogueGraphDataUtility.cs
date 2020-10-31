using System;
using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// A utility class for saving and loading DialogueGraphs
    /// </summary>
    public class DialogueGraphDataUtility
    {
        private readonly DialogueGraphView targetGraphView;
        private IEnumerable<Edge> Edges => targetGraphView == null ? new List<Edge>() : targetGraphView.edges.ToList();
        private List<DialogueNode> dialogueNodes;
        private List<ConditionNode> conditionNodes;
        private EntryNode entryNode;

        public DialogueGraphDataUtility()
        {
            // Default constructor to just save container. Doesn't target a graph;
            targetGraphView = null;
        }

        public DialogueGraphDataUtility(DialogueGraphView targetGraphView)
        {
            this.targetGraphView = targetGraphView;
        }

        public void SaveGraph(DialogueContainer container)
        {
            CacheNodesFromGraph();
            SaveNodes(container);
            SaveNodeLinks(container);

            AssetDatabase.SaveAssets();
        }

        public void SaveGraph(string path, DialogueContainer container)
        {
            CacheNodesFromGraph();
            SaveNodes(container);
            SaveNodeLinks(container);
            WriteAssetFile(container, path);
        }

        public void LoadGraph(DialogueContainer container)
        {
            ClearGraph(container);
            CreateNodes(container);
            ConnectNodes(container);
        }

        private void ClearGraph(DialogueContainer container)
        {
            foreach (Node node in targetGraphView.nodes.ToList())
            {
                // If the entry node, reset the GUID and continue.
                if (node is EntryNode entry)
                {
                    entry.Guid = container.entryNode.guid;
                    entry.SetPosition(new Rect(container.entryNode.position, BaseNode.DefaultNodeSize));
                }
                else // disconnect the node, then remove the node.
                {
                    Edges.Where(edge => edge.input.node == node).ToList()
                        .ForEach(edge => targetGraphView.RemoveElement(edge));

                    targetGraphView.RemoveElement(node);
                }
            }
        }

        /// <summary>
        /// Cache the nodes by looping though them and sorting them based on type
        /// </summary>
        private void CacheNodesFromGraph()
        {
            entryNode = new EntryNode();
            dialogueNodes = new List<DialogueNode>();
            conditionNodes = new List<ConditionNode>();

            if (targetGraphView == null) return;
            foreach (Node node in targetGraphView.nodes.ToList())
            {
                Type nodeType = node.GetType();

                // Sort the nodes into their respective objects.
                if (nodeType == typeof(EntryNode))
                {
                    entryNode = node as EntryNode;
                }
                else if (nodeType == typeof(DialogueNode))
                {
                    dialogueNodes.Add(node as DialogueNode);
                }
                else if (nodeType == typeof(ConditionNode))
                {
                    conditionNodes.Add(node as ConditionNode);
                }
            }
        }

        private void SaveNodes(DialogueContainer container)
        {
            CreateEntryNodeData(container);
            CreateDialogueNodeData(container);
            CreateConditionNodeData(container);
        }

        private void CreateEntryNodeData(DialogueContainer container)
        {
            container.entryNode = new BaseNodeData
            {
                guid = entryNode.Guid,
                position = entryNode.GetPosition().position,
                type = entryNode.type
            };
        }

        private void CreateDialogueNodeData(DialogueContainer container)
        {
            container.dialogueNodeData.Clear();
            // Create DialogueNodeData
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                List<string> responses = new List<string>();
                List<DialogueCondition> conditionsToToggle = new List<DialogueCondition>();

                foreach (DialogueNodePort dialogueNodePort in dialogueNode.DialogueNodePorts)
                {
                    responses.Add(dialogueNodePort.ResponseText);
                    conditionsToToggle.Add(dialogueNodePort.ConditionToToggle);
                }

                container.dialogueNodeData.Add(new DialogueNodeData
                {
                    guid = dialogueNode.Guid,
                    type = dialogueNode.type,
                    position = dialogueNode.GetPosition().position,
                    dialogueText = dialogueNode.DialogueText,
                    speakerName = dialogueNode.SpeakerName,
                    responses = responses,
                    conditionsToToggle = conditionsToToggle
                });
            }
        }

        private void CreateConditionNodeData(DialogueContainer container)
        {
            container.conditionNodeData.Clear();
            foreach (ConditionNode conditionNode in conditionNodes)
            {
                container.conditionNodeData.Add(new ConditionNodeData
                {
                    guid = conditionNode.Guid,
                    position = conditionNode.GetPosition().position,
                    conditionToTest = conditionNode.ConditionToTest,
                    type = conditionNode.type
                });
            }
        }

        private void SaveNodeLinks(DialogueContainer container)
        {
            if (!Edges.Any()) return; // Skip if there are no connections between nodes.

            container.nodeLinkData.Clear();

            // Find all connected nodes
            Edge[] connectedPorts = Edges.Where(port => port.input.node != null).ToArray();

            // Iterate through each connection
            foreach (Edge port in connectedPorts)
            {
                var outputNode = (BaseNode) port.output.node;
                var inputNode = (BaseNode) port.input.node;

                // Add data to list
                container.nodeLinkData.Add(new NodeLinkData
                {
                    baseNodeGuid = outputNode.Guid,
                    baseNodeType = outputNode.type,
                    targetNodeGuid = inputNode.Guid,
                    targetNodeType = inputNode.type
                });

                Debug.Break();
            }
        }

        /// <summary>
        /// Write the asset file to the top level resources folder.
        /// </summary>
        /// <param name="container">The DialogueContainer to write to the asset file.</param>
        /// <param name="path">The name to give the asset file.</param>
        private static void WriteAssetFile(Object container, string path = "Assets/Resources")
        {
            string[] pathArray = path.Split('/');
            string assetName = pathArray[pathArray.Length - 1];
            var inAssetDirectory = false;
            var assetPath = "";

            // Build the absolute path relative to the project root.
            for (var i = 0; i < pathArray.Length - 1; i++)
            {
                if (pathArray[i] == "Assets")
                {
                    inAssetDirectory = true;
                    continue;
                }

                if (!inAssetDirectory) continue;
                assetPath += $"/{pathArray[i]}";
            }

            bool validFolder = AssetDatabase.IsValidFolder("Assets" + assetPath);

            if (!validFolder)
            {
                string[] assetPathArray = assetPath.Split('/');
                for (var i = 1; i < assetPathArray.Length; i++)
                {
                    string parent = i == 1 ? "Assets/" : assetPathArray[i - 1];
                    AssetDatabase.CreateFolder(parent, assetPathArray[i]);
                }
            }

            AssetDatabase.CreateAsset(container, $"Assets/{assetPath}/{assetName}");
            AssetDatabase.SaveAssets();
        }


        private void CreateNodes(DialogueContainer container)
        {
            foreach (DialogueNodeData nodeData in container.dialogueNodeData)
            {
                DialogueNode tempNode = targetGraphView.CreateDialogueNode(nodeData);
                tempNode.Guid = nodeData.guid;
                tempNode.SetPosition(new Rect(nodeData.position, BaseNode.DefaultNodeSize));
                targetGraphView.AddElement(tempNode);
            }

            foreach (ConditionNodeData nodeData in container.conditionNodeData)
            {
                ConditionNode tempNode = targetGraphView.CreateConditionNode(nodeData);
                tempNode.Guid = nodeData.guid;
                tempNode.SetPosition(new Rect(nodeData.position, BaseNode.DefaultNodeSize));
                targetGraphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueContainer container)
        {
            if (!container.nodeLinkData.Any()) return;

            List<BaseNode> nodes = targetGraphView.nodes.ToList().Cast<BaseNode>().ToList();
            foreach (BaseNode node in nodes)
            {
                switch (node)
                {
                    // Connect entry node to targets
                    case EntryNode entry:
                    {
                        string targetNodeGuid = container.nodeLinkData.First(data =>
                            data.baseNodeGuid == entry.Guid).targetNodeGuid;
                        BaseNode targetNode = nodes.First(x => x.Guid == targetNodeGuid);
                        LinkNodes(
                            (Port) entry.outputContainer[0],
                            (Port) targetNode.inputContainer[0]
                        );
                        break;
                    }
                    // Connect dialogue nodes to targets
                    case DialogueNode dialogue:
                    {
                        List<NodeLinkData> dialogueNodeConnections =
                            container.nodeLinkData.Where(data => data.baseNodeGuid == node.Guid).ToList();
                        for (var i = 0; i < dialogueNodeConnections.Count; i++)
                        {
                            string targetNodeGuid = dialogueNodeConnections[i].targetNodeGuid;
                            BaseNode targetNode = nodes.First(n => n.Guid == targetNodeGuid);
                            LinkNodes(
                                dialogue.DialogueNodePorts[i].Port,
                                (Port) targetNode.inputContainer[0]
                            );
                        }

                        break;
                    }
                    // Connect the rest of the nodes to their targets
                    // Works for anything that doesn't add a special way of handling ports.
                    default:
                    {
                        List<NodeLinkData> connections =
                            container.nodeLinkData.Where(data => data.baseNodeGuid == node.Guid).ToList();
                        for (var i = 0; i < connections.Count; i++)
                        {
                            string targetNodeGuid = connections[i].targetNodeGuid;
                            BaseNode targetNode = nodes.First(n => n.Guid == targetNodeGuid);
                            LinkNodes(node.outputContainer[i].Q<Port>(), (Port) targetNode.inputContainer[0]);
                        }

                        break;
                    }
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);

            targetGraphView.Add(tempEdge);
        }
    }
}