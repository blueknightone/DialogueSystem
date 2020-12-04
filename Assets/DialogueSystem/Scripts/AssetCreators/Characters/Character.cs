using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.AssetCreators.Characters
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/New Character", order = 0)]
    public class Character : ScriptableObject
    {
        [SerializeField] private List<CharacterSprite> characterSprites = new List<CharacterSprite>();

        public IEnumerable<CharacterSprite> GetCharacterSprites()
        {
            return characterSprites;
        }

        public void AddSprite(CharacterSprite cSprite)
        {
            characterSprites.Add(cSprite);
            AssetDatabase.AddObjectToAsset(cSprite, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void RemoveSprite(CharacterSprite cSprite)
        {
            AssetDatabase.RemoveObjectFromAsset(cSprite);
            characterSprites.Remove(cSprite);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void RemoveAllSprites()
        {
            for (int i = characterSprites.Count - 1; i >= 0; i--)
            {
                AssetDatabase.RemoveObjectFromAsset(characterSprites[i]);
                characterSprites.RemoveAt(i);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void RenameSubAsset(CharacterSprite cSprite)
        {
            // Remove the old object
            AssetDatabase.RemoveObjectFromAsset(cSprite);
            AssetDatabase.SaveAssets();

            // Add the new object
            AssetDatabase.AddObjectToAsset(cSprite, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}