using System;
using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class ResponseNode : BaseNode
    {
        private DialogueResponse _responseObject;
        private bool _showProps = true;
        private string _newResponseName;
        private string NewResponseName
        {
            get => _newResponseName;
            set
            {
                GUI.changed = true;
                _newResponseName = value;
            }
        }

        public ResponseNode(Rect rect, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<BaseNode> onClickRemoveNode) : base(rect, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onClickRemoveNode)
        {
        }


        public override void Draw(int id)
        {
            Rect = GUILayout.Window(id, Rect, DrawNodeWindow, _responseObject ? _responseObject.name : "Response");
            base.Draw(id);
        }

        private void DrawNodeWindow(int id)
        {
            EditorGUILayout.BeginVertical();

            _responseObject =
                (DialogueResponse) EditorGUILayout.ObjectField(_responseObject, typeof(DialogueResponse), false);

            _showProps = EditorGUILayout.Foldout(_showProps, "Response Properties");
            if (_showProps)
            {
                EditorGUI.indentLevel++;
                if (_responseObject != null)
                {
                    SerializedObject so = new SerializedObject(_responseObject);
                    SerializedProperty responseText = so.FindProperty("responseText");
                    SerializedProperty dialogueCondition = so.FindProperty("dialogueCondition");

                    EditorGUILayout.LabelField("Response Text");
                    EditorGUILayout.PropertyField(responseText, GUIContent.none);
                    EditorGUILayout.LabelField("Condition to Activate");
                    EditorGUILayout.PropertyField(dialogueCondition, GUIContent.none);

                    so.ApplyModifiedProperties();
                }
                else if (_responseObject == null)
                {
                    EditorGUILayout.LabelField("New Response Name");
                    NewResponseName = EditorGUILayout.TextField(NewResponseName);

                    if (string.IsNullOrWhiteSpace(NewResponseName))
                    {
                        EditorGUILayout.HelpBox("An asset or asset name is requires", MessageType.Info);
                    }
                    else if (GUILayout.Button("Add Response"))
                    {
                        _responseObject = ScriptableObject.CreateInstance<DialogueResponse>();
                        AssetDatabase.CreateAsset(_responseObject,
                            AssetDataPath + "Nodes/Response/" + NewResponseName + ".asset");
                    }
                }
                EditorGUI.indentLevel--;
            }


            EditorGUILayout.EndVertical();
        }
    }
}