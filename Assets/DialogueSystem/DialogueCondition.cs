using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Dialogue System/Condition", order = 0)]    
    public class DialogueCondition : ScriptableObject
    {
        [SerializeField] private bool isActive;
        public bool IsActive => isActive;

        public void MakeActive()
        {
            isActive = true;
        }
    }
}