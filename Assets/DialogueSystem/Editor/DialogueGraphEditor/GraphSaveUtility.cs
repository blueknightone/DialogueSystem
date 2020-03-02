using System.Collections.Generic;
using System.Linq;
using lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    public class GraphSaveUtility
    {
        private readonly DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;

        public GraphSaveUtility(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

        private IEnumerable<DialogueNode> DialogueNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(DialogueNode))
            .Cast<DialogueNode>().ToList();
        private List<ConditionNode> ConditionNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(ConditionNode))
            .Cast<ConditionNode>().ToList();

        public void SaveGraph(string filename)
        {
            DialogueContainer dialogueContainer = CreateInstance<DialogueContainer>();
            GetDialogueNodeData(dialogueContainer);
            GetConditionNodeData(dialogueContainer);
            Save(filename, dialogueContainer);
        }

        public void LoadGraph(string filename)
        {
            _containerCache = Resources.Load<DialogueContainer>(filename);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File not found",
                    "Target dialogue graph file does not exist. Please check the file name and try again.", "OK");
                return;
            }

            ClearGraph();
            CreateDialogueNodes();
            CreateConditionNodes();
            ConnectNodes();
        }

        private void GetDialogueNodeData(DialogueContainer dialogueContainer)
        {
            foreach (DialogueNode dialogueNode in DialogueNodes)
            {
                // Skip if the input port isn't connected to anything.
                if (!dialogueNode.inputContainer.Q<Port>().connected) continue;

                // Add new node data to each port
                dialogueContainer.dialogueNodeData.Add(new DialogueNodeData
                {
                    guid = dialogueNode.GUID,
                    dialogueText = dialogueNode.DialogueText,
                    speakerName = dialogueNode.SpeakerName,
                    position = dialogueNode.GetPosition().position
                });

                foreach (ChoicePort choicePort in dialogueNode.ChoicePorts)
                {
                    if (!choicePort.NodePort.connections.Any()) continue; // Skip if the node port has no connections.

                    DialogueNode outputNode = choicePort.NodePort
                        .connections.First().output.node as DialogueNode;
                    BaseNode inputNode = choicePort.NodePort
                        .connections.First().input.node as BaseNode;
                    dialogueContainer.nodeLinkData.Add(new NodeLinkData
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

        private void GetConditionNodeData(DialogueContainer dialogueContainer)
        {
            foreach (ConditionNode conditionNode in ConditionNodes)
            {
                // Skip if the input port isn't connected to anything
                if (!conditionNode.inputContainer.Q<Port>().connected) continue;
                List<Port> conditionPorts = conditionNode.outputContainer.Query<Port>().ToList();
                BaseNode ifTrueTargetNode = conditionPorts[0].connections.First().input.node as BaseNode;
                BaseNode ifFalseTargetNode = conditionPorts[1].connections.First().input.node as BaseNode;

                dialogueContainer.conditionNodeData.Add(new ConditionNodeData
                {
                    guid = conditionNode.GUID,
                    conditionToTest = conditionNode.ConditionToTest,
                    position = conditionNode.GetPosition().position,
                });

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

        private void Save(string filename, DialogueContainer dialogueContainer)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{filename}.asset");
            AssetDatabase.SaveAssets();
        }

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

        private void CreateDialogueNodes()
        {
            foreach (DialogueNodeData nodeData in _containerCache.dialogueNodeData)
            {
                DialogueNode tempNode = _targetGraphView.CreateDialogueNode(BaseNode.DefaultNodeSize, nodeData);
                _targetGraphView.AddElement(tempNode);

                List<NodeLinkData> choicePorts = _containerCache.nodeLinkData.Where(
                    data => data.baseNodeGuid == nodeData.guid).ToList();

                choicePorts.ForEach(portData => tempNode.AddChoicePorts(portData));
            }
        }

        private void CreateConditionNodes()
        {
            foreach (ConditionNodeData nodeData in _containerCache.conditionNodeData)
            {
                ConditionNode tempNode = _targetGraphView.CreateConditionNode(nodeData);
                _targetGraphView.AddElement(tempNode);
            }
        }

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

        private void ConnectStartNode(List<BaseNode> allNodes, List<BaseNodeData> allNodeData)
        {
            _containerCache.nodeLinkData.ForEach(nodeLink =>
            {
                allNodes.ForEach(node =>
                {
                    if (!nodeLink.targetNodeGuid.Contains(node.GUID))
                    {
                        CreatePortConnection(
                            (Port) _targetGraphView.nodes.ToList()[0].outputContainer[0],
                            (Port) node.inputContainer[0]
                        );

                        node.SetPosition(new Rect(
                            allNodeData.First(data => data.guid == node.GUID).position,
                            BaseNode.DefaultNodeSize
                        ));
                    }
                });
            });
        }

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