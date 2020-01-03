using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [RequireComponent(typeof(Canvas))]
    public class ResponseButton : MonoBehaviour
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        public DialogueNode nextConversationNode = null;
        public DialogueUI dialogueUI = null;
        // ReSharper restore RedundantDefaultMemberInitializer

        public void OnClick()
        {
            nextConversationNode.ActivateStoryChoice();
            dialogueUI.conversationNode = nextConversationNode;
            dialogueUI.SetDirty();
        }
    }
}