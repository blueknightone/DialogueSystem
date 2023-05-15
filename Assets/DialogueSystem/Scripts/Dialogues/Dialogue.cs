using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.Characters;
using lastmilegames.DialogueSystem.Dialogues.Nodes;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue System/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<Character> characters = new List<Character>();
        [SerializeField] private List<Node> nodes = new List<Node>();

        private Node currentNode;

        public void ProcessNode(Node node)
        {
            switch (node.Type)
            {
                case NodeType.None:
                    Debug.LogWarning($"No node type selected for {node.name}.");
                    break;
                case NodeType.Dialogue:
                    ProcessDialogueNode(node as DialogueNode);
                    break;
                case NodeType.Condition:
                    ProcessConditionNode(node as ConditionNode);
                    break;
                case NodeType.Event:
                    ProcessEventNode(node as TriggerEventNode);
                    break;
                case NodeType.JumpToNode:
                    break;
                case NodeType.PlayNewDialogue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessDialogueNode(DialogueNode node)
        {
            throw new NotImplementedException();
        }

        private void ProcessConditionNode(ConditionNode node)
        {
            throw new NotImplementedException();
        }

        private void ProcessEventNode(TriggerEventNode node)
        {
            throw new NotImplementedException();
        }
    }
}