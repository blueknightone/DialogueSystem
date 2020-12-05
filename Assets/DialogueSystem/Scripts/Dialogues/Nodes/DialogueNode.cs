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
            return character;
        }

        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private Character character = default;

        [SerializeField] private string dialogue = default;
        // ReSharper restore RedundantDefaultMemberInitializer
    }
}