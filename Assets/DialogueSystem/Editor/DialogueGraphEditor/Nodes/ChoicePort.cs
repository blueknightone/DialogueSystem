using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    public class ChoicePort
    {
        public string GUID { get; }
        public Port NodePort { get; }
        public string ChoiceText { get; set; }
        public DialogueCondition ConditionToToggle { get; set; }

        public ChoicePort(Port nodePort, Action<Node, Port> onClickRemovePort)
        {
            GUID = Guid.NewGuid().ToString();
            NodePort = nodePort;
            
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
                value = false
            };

            TextField choiceTextField = new TextField
            {
                name = string.Empty,
                value = "Choice Text"
            };
            choiceTextField.RegisterValueChangedCallback(evt => { ChoiceText = evt.newValue; });
            foldout.contentContainer.Add(choiceTextField);

            ObjectField dialogueToToggle = new ObjectField("Condition to Toggle")
            {
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