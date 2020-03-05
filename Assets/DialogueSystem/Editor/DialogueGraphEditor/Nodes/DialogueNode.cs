using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * TODO: Note for building UI
 * If a DialogueNode has no ports OR if the port has no edges, display option to end conversation.
 */
namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    /// <summary>
    /// Represents a DialogueNode based off BaseNode.
    /// </summary>
    public class DialogueNode : BaseNode
    {
        /// <summary>
        /// The text to display in the dialogue UI
        /// </summary>
        public string DialogueText { get; private set; }
        
        /// <summary>
        /// The name of the speaker to display in the dialogue UI
        /// </summary>
        public string SpeakerName { get; private set; }
        
        /// <summary>
        /// The ChoicePorts that represent possible responses.
        /// </summary>
        public List<ChoicePort> ChoicePorts { get; } = new List<ChoicePort>();

        /// <summary>
        /// Generates a new, default DialogueNode.
        /// </summary>
        /// <param name="onClickRemovePort">An action delegate called when the remove port button is clicked.</param>
        public DialogueNode(Action<Node, Port> onClickRemovePort) : base(onClickRemovePort)
        {
            // Associate stylesheet
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Add node fields
            BuildNodeControls();
        }

        /// <summary>
        /// Generates a new DialogueNode with pre-existing data.
        /// </summary>
        /// <param name="onClickRemovePort">An action delegate called when the remove port button is clicked.</param>
        /// <param name="nodeData">The node data to populate the node with.</param>
        public DialogueNode(Action<Node, Port> onClickRemovePort, DialogueNodeData nodeData) : base(onClickRemovePort)
        {
            // Associate stylesheet
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Set the properties from the passed in data.
            GUID = nodeData.guid;
            DialogueText = nodeData.dialogueText;
            SpeakerName = nodeData.speakerName;

            // Add node fields
            BuildNodeControls();
        }

        /// <summary>
        /// Builds the node UI.
        /// </summary>
        private void BuildNodeControls()
        {
            // An UIElements.Button that adds new ChoicePorts to the output container.
            Button addChoiceButton = new Button(AddChoicePort) {text = "Add Choice"};
            titleButtonContainer.Add(addChoiceButton);

            // The UIElements.Foldout that contains the main controls.
            Foldout contentFoldout = new Foldout
            {
                text = "Dialogue Properties"
            };
            contentContainer.Add(contentFoldout);

            // The UIElements.TextField that sets the speaker's name.
            TextField speakerNameField = new TextField("Speaker Name")
            {
                value = SpeakerName
            };
            speakerNameField.RegisterValueChangedCallback(evt =>
            {
                SpeakerName = evt.newValue;
                UpdateTitle();
            });
            contentFoldout.contentContainer.Add(speakerNameField);
            
            contentFoldout.contentContainer.Add(new VisualElement {name = "divider"});

            // The UIElements.TextField that contains the speaker's dialogue
            TextField dialogueTextField = new TextField("Dialogue Text")
            {
                value = DialogueText
            };
            dialogueTextField.RegisterValueChangedCallback(evt =>
            {
                DialogueText = evt.newValue;
                UpdateTitle();
            });
            contentFoldout.contentContainer.Add(dialogueTextField);
            
            contentFoldout.contentContainer.Add(new VisualElement {name = "divider"});

            RefreshExpandedState();
            RefreshPorts();
            SetPosition(new Rect(Vector2.zero, DefaultNodeSize));
        }

        // Adds an empty ChoicePort to the node's output container. 
        private void AddChoicePort()
        {
            ChoicePort choicePort = new ChoicePort(
                GeneratePort(this, "Out", Direction.Output, type: typeof(string)),
                OnClickRemovePort);

            ChoicePorts.Add(choicePort);
            outputContainer.Add(choicePort.NodePort);
            RefreshExpandedState();
            RefreshPorts();
        }

        // Adds a pre-populated ChoicePort to the node's output container.
        public void AddChoicePorts(NodeLinkData portData)
        {
            ChoicePort choicePort = new ChoicePort(
                GeneratePort(this, "Out", Direction.Output, type: typeof(string)),
                OnClickRemovePort,
                portData);

            ChoicePorts.Add(choicePort);
            outputContainer.Add(choicePort.NodePort);
            
            RefreshExpandedState();
            RefreshPorts();
        }

        /// <summary>
        /// Updates the node's title when the form's state changes.
        /// </summary>
        public void UpdateTitle()
        {
            string speakerName;
            if (string.IsNullOrWhiteSpace(SpeakerName))
            {
                speakerName = "NO-SPK";
            }
            else
            {
                speakerName = SpeakerName.Length <= 10
                    ? SpeakerName
                    : SpeakerName.Substring(0, 10) + "...";
            }

            string dialogueText;
            if (string.IsNullOrWhiteSpace(DialogueText))
            {
                dialogueText = "NO-DLG";
            }
            else
            {
                dialogueText = DialogueText.Length <= 12
                    ? DialogueText
                    : DialogueText.Substring(0, 12) + "...";
            }

            title = $"{speakerName} : {dialogueText}";
        }
    }
}