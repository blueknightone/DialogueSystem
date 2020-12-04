using System.Linq;
using lastmilegames.DialogueSystem.AssetCreators.Characters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.AssetCreators
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : Editor
    {
        private Character character;
        private VisualElement cSpriteList;
        private VisualElement rootElement;

        private void OnEnable()
        {
            rootElement = new VisualElement();
            character = (Character) target;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/AssetCreators/Editor/CharacterEditor.uxml");
            visualTree.CloneTree(rootElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/AssetCreators/Editor/CharacterEditor.uss");
            rootElement.styleSheets.Add(styleSheet);
        }

        public override VisualElement CreateInspectorGUI()
        {
            #region Fields

            var characterName = rootElement.Q<TextField>("characterName");
            characterName.isDelayed = true;
            characterName.value = character.name;
            characterName.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                character.name = evt.newValue;
                Utilities.RenameAsset(character, evt.newValue);


                EditorUtility.SetDirty(character);
            });

            #endregion

            #region Diaplay Sprite List

            cSpriteList = rootElement.Q<VisualElement>("spriteList");
            UpdateSpriteList();

            #endregion

            #region Buttons

            var btnAddSprite = rootElement.Q<Button>("btnAddSprite");
            btnAddSprite.clickable.clicked += AddCharacterSprite;

            var btnRemoveAll = rootElement.Q<Button>("btnRemoveAll");
            btnRemoveAll.clickable.clicked += RemoveAllSprites;

            #endregion

            return rootElement;
        }

        public void RemoveCSprite(CharacterSprite cSprite)
        {
            character.RemoveSprite(cSprite);
            UpdateSpriteList();
        }

        private void RemoveAllSprites()
        {
            if (!EditorUtility.DisplayDialog(
                "Delete All Sprite",
                "Are you sure you want to delete all sprites for this character?",
                "Delete All", "Cancel"))
                return;

            character.RemoveAllSprites();
            UpdateSpriteList();
        }

        private void AddCharacterSprite()
        {
            var cSprite = CreateInstance<CharacterSprite>();
            cSprite.name = "New Sprite";
            character.AddSprite(cSprite);
            UpdateSpriteList();
        }

        private void UpdateSpriteList()
        {
            cSpriteList.Clear();
            if (!character.GetCharacterSprites().Any())
            {
                var infoBox = new Box();
                infoBox.Add(new Label
                {
                    text = "No sprites found.",
                    style =
                    {
                        paddingTop = 10, paddingRight = 20, paddingBottom = 10, paddingLeft = 20,
                        unityTextAlign = TextAnchor.MiddleCenter
                    }
                });
                cSpriteList.Add(infoBox);
            }

            foreach (CharacterSprite cSprite in character.GetCharacterSprites())
            {
                var element = new CharacterSpriteElement(this, cSprite);
                cSpriteList.Add(element);
                element.Q<TextField>("spriteName").Focus();
            }
        }

        public void UpdateSubAssetName(CharacterSprite cSprite)
        {
            character.RenameSubAsset(cSprite);
        }
    }
}