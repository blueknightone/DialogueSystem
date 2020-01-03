using System;
using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue Node", order = 0)]
    public class DialogueNode : ScriptableObject
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [Header("Speaker"), Space] 
        
        [SerializeField] private Sprite speakerImage = null;
        [SerializeField] private string speakerName = null;

        [Header("Response"), Space] 
        
        [SerializeField, Tooltip("Ignore this conversation option once it has been read.")]
        private bool ignoreOnceRead = false;
        
        [SerializeField, Tooltip("What the button to select this node will say.")]
        private string responseLabel = null;
        
        [SerializeField, Tooltip("The next nodes in the branch.")]
        private List<DialogueNode> responses = null;

        [Space]
        [Header("Conversation"), Space]
        
        [SerializeField, Tooltip("This story choice will be toggled on or off during this dialogue.")]
        private StoryChoice choiceToActivate = null;
        [SerializeField, Tooltip("The story choice that determines whether to show flavor text or default text.")]
        private StoryChoice storyChoice = null;

        [SerializeField, TextArea(5, 10),
         Tooltip("This is what the speaker will say if there is no story choice or story choice is false.")]
        private string defaultSpeakerDialogue = null;

        [SerializeField, TextArea(5, 10),
         Tooltip("This is what the speaker will say when the story choice has been activated.")]
        private string speakerDialogueWhenTrue = null;
        // ReSharper disable RedundantDefaultMemberInitializer

        public Sprite SpeakerImage => speakerImage;
        public string SpeakerName => speakerName;
        public string ResponseLabel => responseLabel;
        public List<DialogueNode> Responses => responses;
        public bool IgnoreOnceRead => ignoreOnceRead;
        public bool Read { get; private set; }

        private void OnEnable()
        {
            Read = false;
        }

        /// <summary>
        /// Activate the story choice associated with the conversation node
        /// </summary>
        public void ActivateStoryChoice()
        {
            if (choiceToActivate != null)
            {
                choiceToActivate.ActivateStoryChoice();
            }
        }

        /// <summary>
        /// Return the appropriate dialogue based on the story choice.
        /// </summary>
        /// <returns>Returns speakerDialogueWhenTrue string when storyChoice.choiceActive</returns>
        public string GetStoryDialogue()
        {
            Read = true;
            return storyChoice && storyChoice.Value ? speakerDialogueWhenTrue : defaultSpeakerDialogue;
        }
    }
}