using System;
using System.Collections.Generic;
using lastmilegames.DialogueSystem.NodeData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lastmilegames.DialogueSystem
{
    public class DialoguePlayer : MonoBehaviour, IDialoguePlayer
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private Transform dialoguePanel = default;
        [SerializeField] private TMP_Text speakerNameLabel = default;
        [SerializeField] private TMP_Text dialogueLabel = default;
        [SerializeField] private Transform choicesContentAreaTransform = default;
        [SerializeField] private Button choiceButtonPrefab = default;

        [SerializeField] private string closeConversationLabelText = "[End Conversation]";
        // ReSharper restore RedundantDefaultMemberInitializer

        private DialogueContainer dialogueContainer;

        private void Awake()
        {
            dialoguePanel.gameObject.SetActive(false);
        }

        public void PlayDialogue(DialogueContainer container)
        {
            if (container == null)
            {
                Debug.LogError("No dialogue provided by trigger. Aborting conversation.");
                return;
            }

            dialogueContainer = container;

            BaseNodeData startingNode = GetTargetNode(dialogueContainer.nodeLinkData.Find(data =>
                data.baseNodeGuid == dialogueContainer.entryNode.guid));

            PlayNode(startingNode);
            dialoguePanel.gameObject.SetActive(true);
        }

        private BaseNodeData GetTargetNode(NodeLinkData nodeLink)
        {
            switch (nodeLink.targetNodeType)
            {
                case NodeType.NotSet:
                    return null;
                case NodeType.Entry:
                    return null;
                case NodeType.Condition:
                    return dialogueContainer.conditionNodeData.Find(
                        node => node.guid == nodeLink.targetNodeGuid);
                case NodeType.Dialogue:
                    return dialogueContainer.dialogueNodeData.Find(node =>
                        node.guid == nodeLink.targetNodeGuid);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayNode(BaseNodeData node)
        {
            switch (node.type)
            {
                case NodeType.NotSet:
                    break;
                case NodeType.Entry:
                    break;
                case NodeType.Condition:
                    EvaluateConditionNode(node as ConditionNodeData);
                    break;
                case NodeType.Dialogue:
                    DisplayDialogueNode(node as DialogueNodeData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DisplayDialogueNode(DialogueNodeData node)
        {
            speakerNameLabel.text = node.speakerName;
            dialogueLabel.text = node.dialogueText;

            ClearChoiceButtons();
            
            if (node.responses.Count == 0)
            {
                Button endButton = Instantiate(choiceButtonPrefab, choicesContentAreaTransform);
                endButton.GetComponentInChildren<TMP_Text>().text = closeConversationLabelText;
                endButton.onClick.AddListener(() => dialoguePanel.gameObject.SetActive(false));
            }
            else
            {
                GenerateChoiceButtons(node);
            }

        }

        private void ClearChoiceButtons()
        {
            foreach (Transform child in choicesContentAreaTransform)
            {
                Destroy(child.gameObject);
            }
        }

        private void GenerateChoiceButtons(DialogueNodeData node)
        {
            List<NodeLinkData> nodeLinks = GetLinks(node.guid);

            if (nodeLinks.Count != node.responses.Count)
            {
                Debug.LogError(
                    $"Dialogue Node {node.guid} has mismatched number of links and responses. Aborting conversation.");
                dialoguePanel.gameObject.SetActive(false);
                return;
            }

            for (var i = 0; i < node.responses.Count; i++)
            {
                NodeLinkData link = nodeLinks[i];
                string currentResponse = node.responses[i];
                DialogueCondition conditionToToggle = node.conditionsToToggle[i];
                Button button = Instantiate(choiceButtonPrefab, choicesContentAreaTransform);
                button.GetComponentInChildren<TMP_Text>().text = currentResponse;
                button.onClick.AddListener(() => OnButtonClick(link, conditionToToggle));
            }
        }

        private void OnButtonClick(NodeLinkData linkData, DialogueCondition conditionToToggle)
        {
            if (conditionToToggle != null) conditionToToggle.ToggleValue();
            
            BaseNodeData nextNode = GetTargetNode(linkData);
            
            PlayNode(nextNode);
        }

        private void EvaluateConditionNode(ConditionNodeData node)
        {
            List<NodeLinkData> nextNodes = GetLinks(node.guid);

            BaseNodeData nextNode = GetTargetNode(node.conditionToTest.Value ? nextNodes[0] : nextNodes[1]);
            
            PlayNode(nextNode);
        }

        private List<NodeLinkData> GetLinks(string guid)
        {
            return dialogueContainer.nodeLinkData.FindAll(link => link.baseNodeGuid == guid);
        }
    }
}