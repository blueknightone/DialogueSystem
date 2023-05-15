using UnityEngine;

namespace lastmilegames.DialogueSystem.Conditions
{
    [CreateAssetMenu(fileName = "New Condition", menuName = "Dialogue System/New Condition", order = 0)]
    public class Condition : ScriptableObject
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField] private bool initialValue = false;
        // ReSharper restore RedundantDefaultMemberInitializer

#if UNITY_EDITOR
        public string developerNotes = "";
#endif
        public bool InitialValue => initialValue;

        public void ToggleValue()
        {
            initialValue = !initialValue;
        }

        public void SetValue(bool newValue)
        {
            initialValue = newValue;
        }

        public void SetValue(Condition newValue)
        {
            initialValue = newValue.initialValue;
        }
    }
}