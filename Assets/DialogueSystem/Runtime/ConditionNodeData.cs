using System;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [Serializable]
    public class ConditionNodeData
    {
        public string guid;
        public DialogueCondition conditionToTest;
        public Vector2 position;
        public string ifTrueTargetGuid;
        public string ifFalseTargetGuid;
    }
}