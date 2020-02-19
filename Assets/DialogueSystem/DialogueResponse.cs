using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Response", menuName = "Dialogue System/Response", order = 0)]
    public class DialogueResponse : ScriptableObject
    {
        [SerializeField] private string responseText;
        public string ResponseText => responseText;

        [SerializeField] private DialogueCondition dialogueCondition;
        public DialogueCondition DialogueCondition => dialogueCondition;

        [SerializeField] private Dialogue nextDialogue;
        public Dialogue NextDialogue => nextDialogue;
    }
}