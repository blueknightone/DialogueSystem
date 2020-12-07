using lastmilegames.DialogueSystem.Characters;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CreateAssetMenu]
    public class DialogueNode : Node
    {
        protected override void OnEnable()
        {
            type = NodeType.Dialogue;
        }

        /// <summary>
        /// Gets the dialogue text of the node.
        /// </summary>
        /// <returns>Returns the text to display in the UI.</returns>
        public string GetDialogueText()
        {
            return dialogue;
        }

        /// <summary>
        /// Gets the "speaker" for the dialogue node.
        /// </summary>
        /// <returns>Returns a <code>Character</code> object with the character data.</returns>
        public Character GetCharacter()
        {
            return speaker;
        }

        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private Character speaker = default;

        [SerializeField] private string dialogue = default;
        // ReSharper restore RedundantDefaultMemberInitializer

#if UNITY_EDITOR
        /// <summary>
        /// Updates the speaker for the node.
        /// </summary>
        /// <param name="newSpeaker">A DialogueSystem.Character to associate with the dialogue.</param>
        /// <remarks>Editor only method.</remarks>
        public void SetSpeaker(Character newSpeaker)
        {
            speaker = newSpeaker;
        }

        /// <summary>
        /// Update the dialogue text to the new string.
        /// </summary>
        /// <param name="text">The new dialogue text.</param>
        /// <remarks>Editor only method.</remarks>
        public void SetDialogueText(string text)
        {
            dialogue = text;
        }
#endif
    }
}