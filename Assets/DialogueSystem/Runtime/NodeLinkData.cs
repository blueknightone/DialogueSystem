using System;
using System.Collections.Generic;

namespace lastmilegames.DialogueSystem
{
    [Serializable]
    public class NodeLinkData
    {
        public string baseNodeGuid;
        public string choiceText;
        public DialogueCondition dialogueCondition;
        public List<string> targetNodeGuid;
    }
}