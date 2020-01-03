using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    public class EndConversationButton : MonoBehaviour
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        public DialogueUI dialogueUI = null;
        // ReSharper restore RedundantDefaultMemberInitializer

        public void OnClick()
        {
            dialogueUI.EndConversation();
        }
    }
}