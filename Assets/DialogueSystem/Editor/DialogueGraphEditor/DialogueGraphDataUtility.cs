using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// A utility class for saving and loading DialogueGraphs
    /// </summary>
    public class DialogueGraphDataUtility
    {
        private DialogueGraphView _targetGraphView;
        private IEnumerable<Edge> Edges => _targetGraphView == null ? new List<Edge>() : _targetGraphView.edges.ToList();
        private List<DialogueNode> _dialogueNodes;
        private List<ConditionNode> _conditionNodes;
        private EntryNode _entryNode;

        public DialogueGraphDataUtility()
        {
            // Default constructor to just save container. Doesn't target a graph;
            _targetGraphView = null;
        }
        
        public DialogueGraphDataUtility(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

        public void SetTargetGraphView(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

        public void SaveGraph(string fileName)
        {
            DialogueContainer container = ScriptableObject.CreateInstance<DialogueContainer>();

            CacheNodesFromGraph();
            SaveNodes(container);
            SaveNodeLinks(container);
            WriteAssetFile(container, fileName);
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

        /// <summary>
        /// Cache the nodes by looping though them and sorting them based on type
        /// </summary>
        private void CacheNodesFromGraph()
        {
            _entryNode = new EntryNode();
            _dialogueNodes = new List<DialogueNode>();
            _conditionNodes = new List<ConditionNode>();
            
            if (_targetGraphView == null) return;
            foreach (Node node in _targetGraphView.nodes.ToList())
            {
                Type nodeType = node.GetType();

                // Sort the nodes into their respective objects.
                if (nodeType == typeof(EntryNode))
                {
                    _entryNode = node as EntryNode;
                }
                else if (nodeType == typeof(DialogueNode))
                {
                    _dialogueNodes.Add(node as DialogueNode);
                }
                else if (nodeType == typeof(ConditionNode))
                {
                    _conditionNodes.Add(node as ConditionNode);
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
                baseNodeGUID = _entryNode.GUID,
                position = _entryNode.GetPosition().position
            };
        }

        private void CreateDialogueNodeData(DialogueContainer container)
        {
            container.dialogueNodeData.Clear();
            // Create DialogueNodeData
            foreach (DialogueNode dialogueNode in _dialogueNodes)
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
                    baseNodeGUID = dialogueNode.GUID,
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
            foreach (ConditionNode conditionNode in _conditionNodes)
            {
                container.conditionNodeData.Add(new ConditionNodeData
                {
                    baseNodeGUID = conditionNode.GUID,
                    position = conditionNode.GetPosition().position,
                    conditionToTest = conditionNode.ConditionToTest
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
                BaseNode outputNode = port.output.node as BaseNode;
                BaseNode inputNode = port.input.node as BaseNode;

                // Add data to list
                container.nodeLinkData.Add(new NodeLinkData
                {
                    baseNodeGuid = outputNode?.GUID,
                    targetNodeGuid = inputNode?.GUID
                });
            }
        }

        /// <summary>
        /// Write the asset file to the top level resources folder.
        /// </summary>
        /// <param name="container">The DialogueContainer to write to the asset file.</param>
        /// <param name="path">The name to give the asset file.</param>
        private void WriteAssetFile(DialogueContainer container, string path = "Assets/Resources")
        {
            string[] pathArray = path.Split('/');
            string assetName = pathArray[pathArray.Length - 1];
            var inAssetDirectory = false;
            var assetPath = "";

            // Build the absolute path relative to the project root.
            for (var i = 0; i < pathArray.Length -1; i++)
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


        public void LoadGraph(string fileName)
        {
            DialogueContainer container = Resources.Load<DialogueContainer>(fileName);

            if (container == null)
            {
                EditorUtility.DisplayDialog("File Not Found",
                    "Target dialogue graph file does not exists!\nCheck the filename and try again.", "OK");
            }

            ClearGraph(container);
            CreateNodes(container);
            ConnectNodes(container);
        }

        public void LoadGraph(DialogueContainer container)
        {
            ClearGraph(container);
            CreateNodes(container);
            ConnectNodes(container);
        }

        private void ClearGraph(DialogueContainer container)
        {
            foreach (Node node in _targetGraphView.nodes.ToList())
            {
                // If the entry node, reset the GUID and continue.
                if (node is EntryNode entryNode)
                {
                    entryNode.GUID = container.entryNode.baseNodeGUID;
                    entryNode.SetPosition(new Rect(container.entryNode.position, BaseNode.DefaultNodeSize));
                }
                else // disconnect the node, then remove the node.
                {
                    Edges.Where(edge => edge.input.node == node).ToList()
                        .ForEach(edge => _targetGraphView.RemoveElement(edge));

                    _targetGraphView.RemoveElement(node);
                }
            }
        }

        private void CreateNodes(DialogueContainer container)
        {
            foreach (DialogueNodeData nodeData in container.dialogueNodeData)
            {
                DialogueNode tempNode = _targetGraphView.CreateDialogueNode(nodeData);
                tempNode.GUID = nodeData.baseNodeGUID;
                tempNode.SetPosition(new Rect(nodeData.position, BaseNode.DefaultNodeSize));
                _targetGraphView.AddElement(tempNode);
            }

            foreach (ConditionNodeData nodeData in container.conditionNodeData)
            {
                ConditionNode tempNode = _targetGraphView.CreateConditionNode(nodeData);
                tempNode.GUID = nodeData.baseNodeGUID;
                tempNode.SetPosition(new Rect(nodeData.position, BaseNode.DefaultNodeSize));
                _targetGraphView.AddElement(tempNode);
            }
        }

        private void ConnectNodes(DialogueContainer container)
        {
            if (!container.nodeLinkData.Any()) return;

            List<BaseNode> nodes = _targetGraphView.nodes.ToList().Cast<BaseNode>().ToList();
            foreach (BaseNode node in nodes)
            {
                switch (node)
                {
                    // Connect entry node to targets
                    case EntryNode entryNode:
                    {
                        string targetNodeGuid = container.nodeLinkData.First(data =>
                            data.baseNodeGuid == entryNode.GUID).targetNodeGuid;
                        BaseNode targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                        LinkNodes(
                            (Port) entryNode.outputContainer[0],
                            (Port) targetNode.inputContainer[0]
                        );
                        break;
                    }
                    // Connect dialogue nodes to targets
                    case DialogueNode dialogueNode:
                    {
                        List<NodeLinkData> dialogueNodeConnections =
                            container.nodeLinkData.Where(data => data.baseNodeGuid == node.GUID).ToList();
                        for (int i = 0; i < dialogueNodeConnections.Count; i++)
                        {
                            string targetNodeGuid = dialogueNodeConnections[i].targetNodeGuid;
                            BaseNode targetNode = nodes.First(n => n.GUID == targetNodeGuid);
                            LinkNodes(
                                dialogueNode.DialogueNodePorts[i].Port,
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
                            container.nodeLinkData.Where(data => data.baseNodeGuid == node.GUID).ToList();
                        for (int i = 0; i < connections.Count; i++)
                        {
                            string targetNodeGuid = connections[i].targetNodeGuid;
                            BaseNode targetNode = nodes.First(n => n.GUID == targetNodeGuid);
                            LinkNodes(node.outputContainer[i].Q<Port>(), (Port) targetNode.inputContainer[0]);
                        }

                        break;
                    }
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            Edge tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);

            _targetGraphView.Add(tempEdge);
        }
    }
}