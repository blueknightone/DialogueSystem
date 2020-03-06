using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    /// <summary>
    /// A utility class for saving and loading DialogueGraphs
    /// </summary>
    public class DialogueGraphDataUtility
    {
        /// <summary>
        /// A reference to the DialogueGraphView to save/load from.
        /// </summary>
        private readonly DialogueGraphView _targetGraphView;

        /// <summary>
        /// A cache to temporarily story the loaded data.
        /// </summary>
        private DialogueContainer _containerCache;

        /// <summary>
        /// Creates a new instance of the utility
        /// </summary>
        /// <param name="targetGraphView">The DialogueGraphView to target.</param>
        public DialogueGraphDataUtility(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

        /// <summary>
        /// Stores a list of all DialogueNodes in the target graph.
        /// </summary>
        private IEnumerable<DialogueNode> DialogueNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(DialogueNode))
            .Cast<DialogueNode>().ToList();

        /// <summary>
        /// Stores a list of all the ConditionNodes in the target graph
        /// </summary>
        private List<ConditionNode> ConditionNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(ConditionNode))
            .Cast<ConditionNode>().ToList();

        /// <summary>
        /// Saves the graph data to a DialogueContainer asset.
        /// </summary>
        /// <param name="filename">The filename of the asset.</param>
        public void SaveGraph(string filename)
        {
            DialogueContainer dialogueContainer = CreateInstance<DialogueContainer>();

            // TODO: Store the link data for the entry node.

            // Add DialogueNode data and their links to the container.
            GetDialogueNodeData(dialogueContainer);

            // Add ConditionNode data and their links to the container.
            GetConditionNodeData(dialogueContainer);

            // Write to the file.
            Save(filename, dialogueContainer);
        }

        /// <summary>
        /// Loads the graph data from an asset file.
        /// </summary>
        /// <param name="filename">The name of the asset file to load.</param>
        /// TODO: Load data from an asset selected in project view, like ShaderGraph does.
        public void LoadGraph(string filename)
        {
            // Attempt to cache the data.
            _containerCache = Resources.Load<DialogueContainer>(filename);

            // If the data fails to cache, alert the users.
            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("Error Loading File.",
                    "There was a problem loading the file. Please check the file name and try again.",
                    "OK");
                return;
            }

            ClearGraph(); // Remove an existing nodes and connections.
            CreateDialogueNodes(); // Build the DialogueNodes.
            CreateConditionNodes(); // Build the ConditionNodes.
            ConnectNodes(); // Connect everything together.
        }

        /// <summary>
        /// Finds all the DialogueNodes in the graph and adds their data to the container.
        /// </summary>
        /// <param name="dialogueContainer">The DialogueContainer to write the nodes to.</param>
        private void GetDialogueNodeData(DialogueContainer dialogueContainer)
        {
            foreach (DialogueNode dialogueNode in DialogueNodes)
            {
                // Don't save the node if the input isn't connected.
                if (!dialogueNode.inputContainer.Q<Port>().connected) continue;

                // Add the node's data to the container.
                dialogueContainer.dialogueNodeData.Add(new DialogueNodeData
                {
                    guid = dialogueNode.GUID,
                    dialogueText = dialogueNode.DialogueText,
                    speakerName = dialogueNode.SpeakerName,
                    position = dialogueNode.GetPosition().position
                });

                foreach (ChoicePort choicePort in dialogueNode.ChoicePorts)
                {
                    // Skip if the node port has no connections.
                    if (!choicePort.NodePort.connections.Any()) continue;

                    // Get the input and output nodes.
                    DialogueNode outputNode = choicePort.NodePort.node as DialogueNode;
                    BaseNode inputNode= choicePort.NodePort
                        .connections.First().input.node as BaseNode;
                    
                    // Add the targets to the container.
                    dialogueContainer.nodeLinkData.Add(
                        new NodeLinkData
                        {
                            baseNodeGuid = outputNode?.GUID,
                            choiceText = choicePort.ChoiceText,
                            dialogueCondition = choicePort.ConditionToToggle,
                            targetNodeGuid = new List<string>
                            {
                                inputNode?.GUID
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Finds all the ConditionNodes in the graph and adds their data to the container. 
        /// </summary>
        /// <param name="dialogueContainer">The dialogue container to add data to.</param>
        private void GetConditionNodeData(DialogueContainer dialogueContainer)
        {
            foreach (ConditionNode conditionNode in ConditionNodes)
            {
                // Skip if the input port isn't connected to anything
                if (!conditionNode.inputContainer.Q<Port>().connected) continue;
                
                // Get the connection targets
                List<Port> conditionPorts = conditionNode.outputContainer.Query<Port>().ToList();
                BaseNode ifTrueTargetNode = conditionPorts[0].connections.First().input.node as BaseNode;
                BaseNode ifFalseTargetNode = conditionPorts[1].connections.First().input.node as BaseNode;

                // Add the ConditionNode's data to the container.
                dialogueContainer.conditionNodeData.Add(new ConditionNodeData
                {
                    guid = conditionNode.GUID,
                    conditionToTest = conditionNode.ConditionToTest,
                    position = conditionNode.GetPosition().position
                });

                // Add the connection data to the container
                dialogueContainer.nodeLinkData.Add(new NodeLinkData
                {
                    baseNodeGuid = conditionNode.GUID,
                    dialogueCondition = conditionNode.ConditionToTest,
                    targetNodeGuid = new List<string>
                    {
                        ifTrueTargetNode?.GUID,
                        ifFalseTargetNode?.GUID
                    }
                });
            }
        }

        /// <summary>
        /// Save the dialogueContainer as an asset file.
        /// </summary>
        /// <param name="filename">The name of the file to save to.</param>
        /// <param name="dialogueContainer">The dialogue data to save.</param>
        private static void Save(string filename, DialogueContainer dialogueContainer)
        {
            // TODO: Refactor so that user creates a new DialogueContainer asset and then saves to it.
            
            // Check if save location exists, if not create it.
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            // Create and save teh asset file.
            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{filename}.asset");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Removes all nodes and edges from the graph.
        /// </summary>
        private void ClearGraph()
        {
            List<Node> nodes = _targetGraphView.nodes.ToList();
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                // Disconnect and remove edges from connected node.
                _targetGraphView.edges.ToList().Where(edge => edge.input.node == nodes[i]).ToList()
                    .ForEach(edge =>
                    {
                        edge.output.DisconnectAll();
                        _targetGraphView.RemoveElement(edge);
                    });

                // Remove all nodes except entry node.
                if (i != 0) _targetGraphView.RemoveElement(nodes[i]);
            }
        }

        /// <summary>
        /// Creates the DialogueNodes for the cached data.
        /// </summary>
        private void CreateDialogueNodes()
        {
            foreach (DialogueNodeData nodeData in _containerCache.dialogueNodeData)
            {
                DialogueNode tempNode = _targetGraphView.CreateDialogueNode(nodeData);
                _targetGraphView.AddElement(tempNode);

                List<NodeLinkData> choicePorts = _containerCache.nodeLinkData.Where(
                    data => data.baseNodeGuid == nodeData.guid).ToList();

                choicePorts.ForEach(portData => tempNode.AddChoicePorts(portData));
            }
        }

        /// <summary>
        /// Creates the ConditionNodes from the cached data.
        /// </summary>
        private void CreateConditionNodes()
        {
            foreach (ConditionNodeData nodeData in _containerCache.conditionNodeData)
            {
                ConditionNode tempNode = DialogueGraphView.CreateConditionNode(nodeData);
                _targetGraphView.AddElement(tempNode);
            }
        }

        /// <summary>
        /// Creates the connections between nodes with cached data.
        /// </summary>
        private void ConnectNodes()
        {
            List<BaseNode> allNodes = new List<BaseNode>(DialogueNodes.Count() + ConditionNodes.Count);
            allNodes.AddRange(DialogueNodes);
            allNodes.AddRange(ConditionNodes);
            List<BaseNodeData> allNodeData = new List<BaseNodeData>(_containerCache.dialogueNodeData.Count +
                                                                    _containerCache.conditionNodeData.Count);
            allNodeData.AddRange(_containerCache.dialogueNodeData);
            allNodeData.AddRange(_containerCache.conditionNodeData);

            ConnectStartNode(allNodes, allNodeData);

            foreach (BaseNode baseNode in allNodes)
            {
                List<NodeLinkData> connections = _containerCache.nodeLinkData.Where(
                    data => data.baseNodeGuid == baseNode.GUID).ToList();

                for (int i = 0; i < connections.Count; i++)
                {
                    List<string> targetNodeIDs = connections[i].targetNodeGuid;

                    // Connect Dialogue Nodes
                    if (baseNode.GetType() == typeof(DialogueNode))
                    {
                        DialogueNode baseDialogueNode = baseNode as DialogueNode;
                        BaseNode targetNode =
                            allNodes.First(node => node.GUID == targetNodeIDs[0]);
                        CreatePortConnection(
                            baseDialogueNode?.ChoicePorts[i].NodePort,
                            (Port) targetNode.inputContainer[0]
                        );
                        targetNode.SetPosition(new Rect(
                            allNodeData.First(data => data.guid == targetNodeIDs[0]).position,
                            BaseNode.DefaultNodeSize
                        ));
                    }

                    // Connect CondtionNodes
                    else if (baseNode.GetType() == typeof(ConditionNode))
                    {
                        ConditionNode baseConditionNode = baseNode as ConditionNode;
                        BaseNode ifTrueTargetNode = allNodes.First(node => node.GUID == targetNodeIDs[0]);
                        CreatePortConnection(
                            (Port) baseConditionNode?.outputContainer[0],
                            (Port) ifTrueTargetNode.inputContainer[0]
                        );
                        ifTrueTargetNode.SetPosition(new Rect(
                            new Rect(
                                allNodeData.First(data => data.guid == ifTrueTargetNode.GUID).position,
                                BaseNode.DefaultNodeSize
                            )));

                        BaseNode ifFalseTargetNode = allNodes.First(node => node.GUID == targetNodeIDs[1]);
                        CreatePortConnection(
                            (Port) baseConditionNode?.outputContainer[1],
                            (Port) ifFalseTargetNode.inputContainer[0]
                        );

                        ifFalseTargetNode.SetPosition(new Rect(
                            new Rect(
                                allNodeData.First(data => data.guid == ifFalseTargetNode.GUID).position,
                                BaseNode.DefaultNodeSize
                            )));
                    }
                }
            }
        }

        /// <summary>
        /// Connects the start node to the first nodein the data.
        /// </summary>
        /// <param name="allNodes">All of the node data.</param>
        /// <param name="allNodeData">All the link data.</param>
        private void ConnectStartNode(List<BaseNode> allNodes, IEnumerable<BaseNodeData> allNodeData)
        {
            // TODO: There is a bug here causing all the nodes to be connected to the entry node.
            // Go through each node in the cache and see if it is targeted by another node.
            _containerCache.nodeLinkData.ForEach(nodeLink =>
            {
                allNodes.ForEach(node =>
                {
                    // If the node is targeted by another, move on.
                    if (nodeLink.targetNodeGuid.Contains(node.GUID)) return;
                    
                    // Otherwise, create the connection...
                    CreatePortConnection(
                        (Port) _targetGraphView.nodes.ToList()[0].outputContainer[0],
                        (Port) node.inputContainer[0]
                    );

                    //...and set its position.
                    node.SetPosition(new Rect(
                        allNodeData.First(data => data.guid == node.GUID).position,
                        BaseNode.DefaultNodeSize
                    ));
                });
            });
        }

        /// <summary>
        /// Create a connection between two ports.
        /// </summary>
        /// <param name="output">The output port to connect</param>
        /// <param name="input">The input port to connect</param>
        private void CreatePortConnection(Port output, Port input)
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