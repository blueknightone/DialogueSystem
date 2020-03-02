using System;

namespace lastmilegames.DialogueSystem
{
    [Serializable]
    public class ConditionNodeData : BaseNodeData
    {
        public DialogueCondition conditionToTest;
    }
}