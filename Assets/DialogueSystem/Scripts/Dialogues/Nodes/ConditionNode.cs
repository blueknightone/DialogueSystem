using lastmilegames.DialogueSystem.Conditions;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public class ConditionNode : Node
    {
        [SerializeField] private Condition condition = null;
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
#endif
    }
}