﻿using System;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    /// <summary>
    /// Base class for serializable node data. Represents the data all nodes should track.
    /// </summary>
    [Serializable]
    public abstract class BaseNodeData
    {
        public string guid;
        public Vector2 position;
    }
}