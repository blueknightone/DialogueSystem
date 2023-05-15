using lastmilegames.DialogueSystem.Conditions;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public class ConditionNode : Node
    {
        [SerializeField] private NodeFunction nodeFunction = NodeFunction.Get;
        [SerializeField] private Condition condition = null;
        public NodeFunction NodeFunction => nodeFunction;
        public Condition Condition => condition;

        protected override void OnEnable()
        {
            type = NodeType.Condition;
        }

#if UNITY_EDITOR
        public void SetCondition(Condition newCondition)
        {
            condition = newCondition;
        }

        public void SetFunction(NodeFunction newFunction)
        {
            nodeFunction = newFunction;
        }
#endif
    }
}