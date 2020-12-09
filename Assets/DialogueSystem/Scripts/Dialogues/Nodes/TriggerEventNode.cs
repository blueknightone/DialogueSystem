using UnityEngine;
using UnityEngine.Events;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public class TriggerEventNode : Node
    {
        [SerializeField] private UnityEvent onNodeEntered = new UnityEvent();

        protected override void OnEnable()
        {
            type = NodeType.Event;
        }

        public void Trigger()
        {
            onNodeEntered?.Invoke();
        }
    }
}