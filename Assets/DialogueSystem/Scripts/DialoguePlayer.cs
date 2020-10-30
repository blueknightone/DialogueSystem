using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lastmilegames.DialogueSystem
{
    public class DialoguePlayer : MonoBehaviour, IDialoguePlayer
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private Transform dialoguePanel = default;
        [SerializeField] private TMP_Text speakerNameText = default;
        [SerializeField] private TMP_Text dialogueText = default;
        [SerializeField] private Transform choicesContentAreaTransform = default;
        [SerializeField] private Button choiceButtonPrefab = default;
        // ReSharper restore RedundantDefaultMemberInitializer

        private DialogueContainer _dialogueContainer;
        private List<BaseNodeData> allNodes;
        
        private void Awake()
        {
            dialoguePanel.gameObject.SetActive(false);
        }

        public void PlayDialogue(DialogueContainer container)
        {
            GetAllNodeBaseData(container);

            BaseNodeData startingNode = GetNode(allNodes.First().baseNodeGUID);

            PlayNode(startingNode);
            
            dialoguePanel.gameObject.SetActive(true);
        }

        private void PlayNode(BaseNodeData nodeData)
        {
            // TODO: Implement looking up node.
            // Probably need to move NodeType out of editor and
            // record target node type in base node data.
        }

        private BaseNodeData GetNode(string guid)
        {
            return allNodes.First(data => data.baseNodeGUID == guid);
        }

        private void GetAllNodeBaseData(DialogueContainer container)
        {
            allNodes = new List<BaseNodeData> {container.entryNode};

            foreach (ConditionNodeData nodeData in container.conditionNodeData)
            {
                allNodes.Add(nodeData);
            }

            foreach (DialogueNodeData nodeData in container.dialogueNodeData)
            {
                allNodes.Add(nodeData);
            }
        }
    }
}