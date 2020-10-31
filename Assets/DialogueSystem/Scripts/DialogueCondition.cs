using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// A serializable bool that con be used to branch dialogue.
    /// </summary>
    [CreateAssetMenu(fileName = "Condition", menuName = "Dialogue System/New Condition", order = 0)]
    public class DialogueCondition : ScriptableObject
    {
        /// <summary>
        /// The value of the condition.
        /// </summary>
        /// <remarks>Is automatically set by OnEnable so that value can be changed in play mode without persisting to the object.</remarks>
        public bool Value { get; private set; }
        /// <summary>
        /// The initial value of the
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private bool initialValue = false;
        
        private void OnEnable()
        {
            Value = initialValue;
        }

        /// <summary>
        /// Toggle the runtime value of the condition without affecting the initial value.
        /// </summary>
        public void ToggleValue()
        {
            Value = !Value;
        }
    }
}