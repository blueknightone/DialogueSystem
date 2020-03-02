using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Dialogue System/New Condition", order = 0)]
    public class DialogueCondition : ScriptableObject
    {
        public bool Value { get; private set; }
        public Vector2 Position { get; set; }
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private bool initialValue = false;

        private void OnEnable()
        {
            Value = initialValue;
        }

        public void ToggleValue()
        {
            Value = !Value;
        }
    }
}