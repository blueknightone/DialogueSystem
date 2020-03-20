using System;
using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// A utility class for saving and loading DialogueGraphs
    /// </summary>
    public class DialogueGraphDataUtility
    {
        private DialogueGraphView _targetGraphView;
        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();
        private List<ConditionNode> _conditionNodes = new List<ConditionNode>();
        private EntryNode _entryNode;

        public DialogueGraphDataUtility(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

        public void SaveGraph(string fileName)
        {
            Debug.LogFormat("Saving Graph {0}", fileName);
            
            DialogueContainer container = ScriptableObject.CreateInstance<DialogueContainer>();

            CacheNodesFromGraph();
            SaveNodes(container);
            SaveNodeLinks(container);
            WriteAssetFile(fileName, container);
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
            
            // Find all connected nodes
            Edge[] connectedPorts = Edges.Where(port => port.input.node != null).ToArray();
            
            // Iterate through each connection
            foreach (Edge port in connectedPorts)
            {
                BaseNode outputNode = port.output.node as BaseNode;
                BaseNode inputNode = port.input.node as BaseNode;
                
                container.nodeLinkData.Add(new NodeLinkData
                {
                    baseNodeGuid = outputNode?.GUID,
                    targetNodeGuid = inputNode?.GUID
                });
            }
        }

        private void WriteAssetFile(string filename, DialogueContainer container)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            AssetDatabase.CreateAsset(container, $"Assets/Resources/{filename}.asset");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Cache the nodes by looping though them and sorting them based on type
        /// </summary>
        private void CacheNodesFromGraph()
        {
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

        public void LoadGraph(string fileName)
        {
            Debug.LogFormat("Loading Graph {0}", fileName);
        }
    }
}