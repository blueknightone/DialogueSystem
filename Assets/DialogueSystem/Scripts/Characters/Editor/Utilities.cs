using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Characters
{
    public static class Utilities
    {
        public static void RenameAsset(Object asset, string newName)
        {
            if (!EditorUtility.DisplayDialog(
                "Rename Character",
                "Do you want to change the asset name to match the new character name?",
                "Rename Asset", "Cancel"))
            {
                return;
            }

            string currentPath = AssetDatabase.GetAssetPath(asset);

            string error = AssetDatabase.RenameAsset(currentPath, newName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}