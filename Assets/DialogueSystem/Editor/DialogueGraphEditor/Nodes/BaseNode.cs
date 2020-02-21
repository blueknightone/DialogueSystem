using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor.Nodes
{
    public abstract class BaseNode : Node
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
        public string GUID { get; }
        
        protected readonly Action<Node, Port> OnClickRemovePort;

        /// <summary>
        /// Creates a default node with an input port.
        /// </summary>
        protected BaseNode()
        {
            GUID = Guid.NewGuid().ToString();
            
            GeneratePort(this,"Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Q<Port>().portColor = Color.cyan;
        }

        /// <summary>
        /// Generates a node with an onClickRemovePort event.
        /// </summary>
        /// <param name="onClickRemovePort">Event that fires when remove port button is clicked.</param>
        protected BaseNode(Action<Node, Port> onClickRemovePort)
        {
            GUID = Guid.NewGuid().ToString();
            OnClickRemovePort = onClickRemovePort;
            
            // Create input port
            GeneratePort(this,"Input", Direction.Input, Port.Capacity.Multi);
            inputContainer.Q<Port>().portColor = Color.cyan;
        }
        
        public Port GeneratePort(Node node, string portName, Direction portDirection, Port.Capacity capacity 
            = Port.Capacity.Single, Type type = null)
        {
            Port port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, type);
            port.portName = portName;

            switch (portDirection)
            {
                case Direction.Input:
                    node.inputContainer.Add(port);
                    break;
                case Direction.Output:
                    node.outputContainer.Add(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portDirection), portDirection, null);
            }
            
            return port;
        }
    }
}