using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Utilities
{
    public static class VisualElementExtensions
    {
        public static void ToggleFieldVisibility(this VisualElement field, bool condition)
        {
            field.style.display = condition ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}