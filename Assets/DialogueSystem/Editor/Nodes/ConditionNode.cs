using System;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class ConditionNode : BaseNode
    {
        private DialogueCondition _conditionObject;
        private bool _showProps = true;
        private string _newConditionName;

        public ConditionNode(Rect rect, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<BaseNode> onClickRemoveNode) : base(rect, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onClickRemoveNode)
        {
        }

        public override void Draw(int id)
        {
            Rect = GUILayout.Window(id, Rect, DrawNodeWindow, _conditionObject ? _conditionObject.name : "Condition");
            base.Draw(id);
        }

        private void DrawNodeWindow(int windowId)
        {
            EditorGUILayout.BeginVertical();

            _conditionObject =
                (DialogueCondition) EditorGUILayout.ObjectField(_conditionObject, typeof(DialogueCondition), false);

            _showProps = EditorGUILayout.Foldout(_showProps, "Condition Properties");
            if (_showProps)
            {
                EditorGUI.indentLevel++;
                if (_conditionObject != null)
                {
                    SerializedObject so = new SerializedObject(_conditionObject);
                    SerializedProperty isActive = so.FindProperty("isActive");

                    EditorGUILayout.PropertyField(isActive);

                    so.ApplyModifiedProperties();
                }
                else if (_conditionObject == null)
                {
                    EditorGUILayout.LabelField("New Dialogue Condition");
                    _newConditionName = EditorGUILayout.TextField(_newConditionName);

                    if (string.IsNullOrWhiteSpace(_newConditionName))
                    {
                        EditorGUILayout.HelpBox("An asset name is required.", MessageType.Info, true);
                    }
                    else if (GUILayout.Button("Create Asset"))
                    {
                        _conditionObject = ScriptableObject.CreateInstance<DialogueCondition>();
                        AssetDatabase.CreateAsset(_conditionObject,
                            AssetDataPath + "Nodes/Condition/" + _newConditionName + ".asset");
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
    }
}