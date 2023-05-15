using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public abstract class Node : ScriptableObject, INode
    {
        [SerializeField] protected List<string> children = new List<string>();
        [SerializeField] protected NodeType type = NodeType.None;

        protected abstract void OnEnable();
        public List<string> Children => children;

        public NodeType Type => type;

#if UNITY_EDITOR
        public string DeveloperNotes => developerNotes;
        [SerializeField] private string developerNotes = "";

        public Vector2 Position => position;
        [SerializeField] protected Vector2 position = Vector2.zero;

        public void AddChild(string newChild)
        {
            // Don't let the node connect to something it is already connected to.
            if (children.Contains(newChild)) return;

            // Don't connect to self.
            if (newChild == name) return;

            children.Add(newChild);
        }

        public void RemoveChild(string child)
        {
            children.Remove(child);
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void SetNodeType(NodeType newType)
        {
            type = newType;
        }

        public void SetDevNote(string newNote)
        {
            developerNotes = newNote;
        }
#endif
    }
}