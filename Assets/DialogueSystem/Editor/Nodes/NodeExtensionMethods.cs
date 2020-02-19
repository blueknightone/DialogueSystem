using System;
using UnityEditor.Experimental.GraphView;

namespace lastmilegames.DialogueSystem.Nodes
{
    public static class NodeExtensionMethods
    {
        /// <summary>
        /// Adds an input or output port to a node.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="portName">The name of the port.</param>
        /// <param name="direction">Sets whether the port is an input or output port.</param>
        /// <param name="capacity">Whether the port can accept one connection or multiple connections.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid direction is supplied.</exception>
        public static void GeneratePort(
            this Node node,
            string portName,
            Direction direction,
            Port.Capacity capacity = Port.Capacity.Single
        )
        {
            Port generatedPort = node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
            generatedPort.portName = portName;
            switch (direction)
            {
                case Direction.Input:
                    node.inputContainer.Add(generatedPort);
                    break;
                case Direction.Output:
                    node.outputContainer.Add(generatedPort);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction,
                        "Port must be of type Direction.Input or Direction.Output");
            }
        }
    }
}