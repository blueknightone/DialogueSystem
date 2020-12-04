using lastmilegames.DialogueSystem.AssetCreators.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.AssetCreators
{
    public class CharacterSpriteElement : VisualElement
    {
        private readonly CharacterEditor characterEditor;
        private readonly CharacterSprite characterSprite;

        public CharacterSpriteElement(CharacterEditor characterEditor, CharacterSprite characterSprite)
        {
            this.characterEditor = characterEditor;
            this.characterSprite = characterSprite;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/AssetCreators/Editor/CharacterSpriteElement.uxml");
            visualTree.CloneTree(this);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/AssetCreators/Editor/CharacterSpriteElement.uss");
            styleSheets.Add(styleSheet);
            AddToClassList("characterSprite");

            #region Fields

            var spriteNameField = this.Q<TextField>("spriteName");
            spriteNameField.isDelayed = true;
            spriteNameField.value = characterSprite.name;
            spriteNameField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                characterSprite.name = evt.newValue;
                characterEditor.UpdateSubAssetName(characterSprite);
                EditorUtility.SetDirty(characterSprite);
            });

            var spriteDisplay = this.Q<VisualElement>("spriteDisplay");
            spriteDisplay.style.backgroundImage = characterSprite.sprite ? characterSprite.sprite.texture : null;

            var spriteField = this.Q<ObjectField>("sprite");
            spriteField.objectType = typeof(Sprite);
            spriteField.value = characterSprite.sprite;
            spriteField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                characterSprite.sprite = (Sprite) evt.newValue;
                spriteDisplay.style.backgroundImage = characterSprite.sprite.texture;
                EditorUtility.SetDirty(characterSprite);
            });

            var btnRemoveCSprite = this.Q<Button>("btnRemoveCSprite");
            btnRemoveCSprite.clickable.clicked += RemoveSprite;

            #endregion
        }

        private void RemoveSprite()
        {
            characterEditor.RemoveCSprite(characterSprite);
        }
    }
}