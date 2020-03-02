using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueContainer", menuName = "Dialogue System/New Container", order = 0)]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
        public List<DialogueNodeData> dialogueNodeData = new List<DialogueNodeData>();
        public List<ConditionNodeData> conditionNodeData = new List<ConditionNodeData>();
    }
}