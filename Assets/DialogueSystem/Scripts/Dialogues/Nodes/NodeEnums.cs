namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public enum NodeType
    {
        None,
        Dialogue,
        Condition,
        Event,
        JumpToNode,
        PlayNewDialogue,
    }

    public enum NodeFunction
    {
        Get,
        Set
    }
}