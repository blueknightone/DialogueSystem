using System.Collections.Generic;
using lastmilegames.DialogueSystem.Characters;
using lastmilegames.DialogueSystem.Dialogues.Nodes;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue System/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<Character> characters = new List<Character>();
        [SerializeField] private List<Node> nodes = new List<Node>();
    }
}