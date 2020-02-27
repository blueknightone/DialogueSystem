using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueContainer", menuName = "Dialogue System/New Container", order = 0)]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodes = new List<DialogueNodeData>();
        public List<ConditionNodeData> ConditionNodes = new List<ConditionNodeData>();
    }
}