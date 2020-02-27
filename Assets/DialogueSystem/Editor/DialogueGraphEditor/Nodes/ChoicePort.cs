using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    public class ChoicePort
    {
        public Port NodePort { get; set; }
        public string ChoiceText { get; set; }
        public DialogueCondition ConditionToToggle { get; set; }

        public ChoicePort(Port nodePort, Action<Node, Port> onClickRemovePort)
        {
            NodePort = nodePort;
            
            nodePort.contentContainer.Add(GenerateDeleteButton(onClickRemovePort));
            nodePort.contentContainer.Add(GenerateFoldout());
        }

        public ChoicePort(Port nodePort, Action<Node, Port> onClickRemovePort, NodeLinkData portData)
        {
            NodePort = nodePort;
            ChoiceText = portData.choiceText;
            ConditionToToggle = portData.dialogueCondition;

            nodePort.contentContainer.Add(GenerateDeleteButton(onClickRemovePort));
            nodePort.contentContainer.Add(GenerateFoldout());
        }

        private Button GenerateDeleteButton(Action<Node, Port> onClickRemovePort)
        {
            return new Button(() => onClickRemovePort(NodePort.node, NodePort))
            {
                text = "X",
                tooltip = "Delete Choice"
            };
        }

        private Foldout GenerateFoldout()
        {
            Foldout foldout = new Foldout
            {
                text = "Port Options",
                name = "ChoicePort",
                value = false
            };

            TextField choiceTextField = new TextField
            {
                name = "ChoiceText",
                value = ChoiceText
            };
            choiceTextField.RegisterValueChangedCallback(evt => { ChoiceText = evt.newValue; });
            foldout.contentContainer.Add(choiceTextField);

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