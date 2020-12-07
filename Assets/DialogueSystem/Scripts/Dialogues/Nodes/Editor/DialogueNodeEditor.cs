using lastmilegames.DialogueSystem.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    [CustomEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : NodeEditor
    {
        private DialogueNode dialogueNode;

        protected override VisualElement SetupEditorGUI()
        {
            dialogueNode = (DialogueNode) node;
            var elementRoot = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/DialogueNodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/DialogueNodeEditor.uss");

            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            var objCharacterField = elementRoot.Q<ObjectField>("objCharacterField");
            objCharacterField.objectType = typeof(Character);
            objCharacterField.value = dialogueNode.GetCharacter();
            objCharacterField.RegisterValueChangedCallback(evt =>
                dialogueNode.SetSpeaker((Character) evt.newValue));

            var txtDialogue = elementRoot.Q<TextField>("txtDialogue");
            txtDialogue.value = dialogueNode.GetDialogueText();
            txtDialogue.RegisterValueChangedCallback(evt => dialogueNode.SetDialogueText(evt.newValue));

            return elementRoot;
        }
    }
}