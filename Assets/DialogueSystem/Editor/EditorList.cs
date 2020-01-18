using System;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [Flags]
    public enum EditorListOption
    {
        None = 0, 
        ListSize = 1,
        ListLabel = 2,
        ElementLabels = 4,
        Buttons = 8,
        Default = ListSize | ListLabel | ElementLabels,
        NoElementLabels = ListSize | ListLabel,
        ButtonsNoElementLabels = NoElementLabels | Buttons,
        All = Default | Buttons
    }
    public static class EditorList
    {
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);
        private static readonly GUIContent
            MoveButtonContent = new GUIContent("\u21b4", "Move Down"),
            DuplicateButtonContent = new GUIContent("+", "Duplicate"),
            DeleteButtonContent = new GUIContent("-", "Delete"),
            AddButtonContent = new GUIContent("+", "Add Element");
        
        public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default)
        {
            if (!list.isArray)
            {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }
            bool showListLabel = (options & EditorListOption.ListLabel) != 0,
                showListSize = (options & EditorListOption.ListSize) != 0;
            
            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list, false);
            }
            EditorGUI.indentLevel += 1;
            
            if (!showListLabel || list.isExpanded)
            {
                SerializedProperty size = list.FindPropertyRelative("Array.size");
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(size);
                }

                if (size.hasMultipleDifferentValues)
                {
                    EditorGUILayout.HelpBox("Not showing lists with different sizes", MessageType.Info);
                }
                else
                {
                    ShowElements(list, options);
                }
            }

            if (showListLabel)
            {
                EditorGUI.indentLevel -= 1;
            }
        }

        private static void ShowElements(SerializedProperty list, EditorListOption options)
        {
            bool showElementLabels = (options & EditorListOption.ElementLabels) != 0,
                showButtons = (options & EditorListOption.Buttons) != 0;
            
            for (int i = 0; i < list.arraySize; i++)
            {
                if (showButtons)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                if (showElementLabels)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
                else
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                }

                if (showButtons)
                {
                    ShowButtons(list, i);
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (showButtons && list.arraySize == 0 && GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
        }

        private static void ShowButtons(SerializedProperty list, int index)
        {
            if (GUILayout.Button(MoveButtonContent, EditorStyles.miniButtonLeft, MiniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }

            if (GUILayout.Button(DuplicateButtonContent, EditorStyles.miniButtonMid, MiniButtonWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }

            if (GUILayout.Button(DeleteButtonContent, EditorStyles.miniButtonRight, MiniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }
    }
}