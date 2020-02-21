using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class ConditionNode : BaseNode
    {
        public bool Condition { get; set; } // TODO: Create from scriptable object

        public ConditionNode()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            BuildNodeControls();
        }

        private void BuildNodeControls()
        {
            Port ifTruePort = GeneratePort(this,"If True", Direction.Output);
            ifTruePort.portColor = Color.green;

            Port ifFalsePort = this.GeneratePort(this,"If False", Direction.Output);
            ifFalsePort.portColor = Color.red;
            
            Toggle toggle = new Toggle("Condition Temp");
            contentContainer.Add(toggle);
        }
    }
}