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
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;
        private IEnumerable<DialogueNode> DialogueNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(DialogueNode))
            .Cast<DialogueNode>().ToList();
        private List<ConditionNode> ConditionNodes => _targetGraphView.nodes.ToList()
            .FindAll(node => node.GetType() == typeof(ConditionNode))
            .Cast<ConditionNode>().ToList();

        public GraphSaveUtility(DialogueGraphView targetGraphView)
        {
            _targetGraphView = targetGraphView;
        }

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
                dialogueContainer.DialogueNodes.Add(new DialogueNodeData
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
                    dialogueContainer.NodeLinks.Add(new NodeLinkData
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

                dialogueContainer.ConditionNodes.Add(new ConditionNodeData
                {
                    guid = conditionNode.GUID,
                    conditionToTest = conditionNode.ConditionToTest,
                    position = conditionNode.GetPosition().position,
                });

                dialogueContainer.NodeLinks.Add(new NodeLinkData
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
            foreach (DialogueNodeData nodeData in _containerCache.DialogueNodes)
            {
                DialogueNode tempNode = _targetGraphView.CreateDialogueNode(_targetGraphView.DefaultNodeSize, nodeData);
                _targetGraphView.AddElement(tempNode);

                List<NodeLinkData> choicePorts = _containerCache.NodeLinks.Where(
                    data => data.baseNodeGuid == nodeData.guid).ToList();
                
                choicePorts.ForEach(portData => tempNode.AddChoicePorts(portData));
                
            }
        }

        private void CreateConditionNodes()
        {
            // TODO
        }

        private void ConnectNodes()
        {
            // TODO
        }
    }
}