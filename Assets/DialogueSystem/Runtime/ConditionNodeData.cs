using System;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// Represents the data that a ConditionNode should save.
    /// </summary>
    [Serializable]
    public class ConditionNodeData : BaseNodeData
    {
        public DialogueCondition conditionToTest;
    }
}