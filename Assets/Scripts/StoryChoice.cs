using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "StoryChoice", menuName = "Dialogue/New Story Choice Node", order = 0)]
    public class StoryChoice : ScriptableObject
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private bool initialValue = false;
        // ReSharper restore RedundantDefaultMemberInitializer

        /// <summary>
        /// Runtime value. Returns true if the choice node has been activated. 
        /// </summary>
        public bool Value { get; private set; }

        private void OnEnable()
        {
            Value = initialValue; // Set the runtime value when the object is first loaded. 
        }

        /// <summary>
        /// Saves the runtime value as the default state.
        /// </summary>
        public void ApplyState()
        {
            initialValue = Value;
        }

        // Toggle the state of the story choice.
        public void ActivateStoryChoice()
        {
            Value = true;
        }
    }
}