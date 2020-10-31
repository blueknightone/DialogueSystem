using System;

namespace lastmilegames.DialogueSystem.NodeData
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