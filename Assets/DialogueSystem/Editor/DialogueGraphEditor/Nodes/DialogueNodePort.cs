using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    /// <summary>
    /// A foldout that contains a TextField, ObjectField and GraphView.Port
    /// </summary>
    public class DialogueNodePort
    {
        /// <summary>
        /// A reference to the output GraphView.Port 
        /// </summary>
        public Port Port { get; }
        
        /// <summary>
        /// The ResponseText to display on the response buttons.
        /// </summary>
        public string ResponseText { get; private set; }
        
        /// <summary>
        /// The dialogue condition to toggle the value of when the choice is selected.
        /// </summary>
        public DialogueCondition ConditionToToggle { get; private set; }

        /// <summary>
        /// Generates a default dialogue node port.
        /// </summary>
        /// <param name="port">A GraphView.Port to reference.</param>
        /// <param name="onClickRemovePort">An action delegate that is called when the remove port button is clicked</param>
        public DialogueNodePort(Port port, Action<Node, Port> onClickRemovePort)
        {
            Port = port;
            
            port.contentContainer.Add(GenerateDeleteButton(onClickRemovePort));
            port.contentContainer.Add(GenerateFoldout());
        }

        /// <summary>
        /// Creates a new UIElements.Button for deleting a story choice.
        /// </summary>
        /// <param name="onClickRemovePort">The action delegate to be called when the delete button is pressed.</param>
        /// <returns>Returns a UIElement.Button</returns>
        private Button GenerateDeleteButton(Action<Node, Port> onClickRemovePort)
        {
            return new Button(() => onClickRemovePort(Port.node, Port))
            {
                text = "X",
                tooltip = "Delete Choice"
            };
        }

        /// <summary>
        /// Creates a UIElements.Foldout and adds the DialogueNodePort elements.
        /// </summary>
        /// <returns>Returns a UIElements.Foldout with the DialogueNodePort fields.</returns>
        private Foldout GenerateFoldout()
        {
            // The containing foldout
            Foldout foldout = new Foldout
            {
                text = "Port Options",
                name = "DialogueNodePort",
                value = false
            };

            // The text to display on the response buttons in the game UI.
            TextField choiceTextField = new TextField
            {
                name = "Response Text",
                value = ResponseText
            };
            // Sets the ResponseText property to the value entered in the TextField.
            choiceTextField.RegisterValueChangedCallback(evt => { ResponseText = evt.newValue; });
            foldout.contentContainer.Add(choiceTextField);

            // Sets the dialogue that needs to be toggled when the StoryChoice is chosen in the UI.
            ObjectField dialogueToToggle = new ObjectField("Condition to Toggle")
            {
                name  = "ConditionToToggle",
                objectType = typeof(DialogueCondition),
                allowSceneObjects = false
            };
            dialogueToToggle.RegisterValueChangedCallback(evt =>
            {
                ConditionToToggle = evt.newValue as DialogueCondition;
            });
            foldout.contentContainer.Add(dialogueToToggle);
            
            return foldout;
        }
    }
}