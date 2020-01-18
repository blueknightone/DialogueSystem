using UnityEditor;
using UnityEngine;

namespace lastmilegames.DialogueSystem.Nodes
{
    public class DialogueNode : BaseNode
    {
        private Dialogue _dialogueObject;
        private Vector2 _scrollPos;
        private string _newDialogueName;
        private string NewDialogueName
        {
            get => _newDialogueName;
            set
            {
                GUI.changed = true;
                _newDialogueName = value;
            }
        }
        
        public DialogueNode(Vector2 position, float width, float height) : base(position, width, height)
        {
        }

        public override void Draw(int id)
        {
            string windowTitle = _dialogueObject ? _dialogueObject.name : "Dialogue";
            GUILayout.Window(id, nodeRect, DrawNodeWindow, windowTitle);
        }

        private void DrawNodeWindow(int id)
        {
            EditorGUILayout.BeginVertical();

            _dialogueObject = (Dialogue) EditorGUILayout.ObjectField(_dialogueObject, typeof(Dialogue), false);
            EditorGUILayout.Separator();

            if (_dialogueObject != null)
            {
                SerializedObject so = new SerializedObject(_dialogueObject);
                SerializedProperty speakerText = so.FindProperty("speakerText");
                SerializedProperty bgSpeakerImages = so.FindProperty("backgroundSpeakerImages");
                SerializedProperty speakerImage = so.FindProperty("speakerImage");
                
                EditorGUILayout.LabelField("Speaker Text");
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MinHeight(100f));
                speakerText.stringValue = EditorGUILayout.TextArea(speakerText.stringValue, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();

                EditorGUILayout.LabelField("Speaker Image");
                EditorGUILayout.PropertyField(speakerImage, GUIContent.none, false);
                
                EditorList.Show(bgSpeakerImages, EditorListOption.ButtonsNoElementLabels);
                
                so.ApplyModifiedProperties();
            }
            else if (_dialogueObject == null)
            {
                EditorGUILayout.LabelField("New Dialogue Name");
                NewDialogueName = EditorGUILayout.TextField(NewDialogueName);

                if (string.IsNullOrWhiteSpace(NewDialogueName))
                {
                    EditorGUILayout.HelpBox("An asset name is required.", MessageType.Info, true);
                }
                else
                {
                    if (GUILayout.Button("Create Asset"))
                    {
                        _dialogueObject = CreateInstance<Dialogue>();
                        AssetDatabase.CreateAsset(_dialogueObject,
                            AssetDataPath + "Nodes/Dialogue/" + _newDialogueName + ".asset");
                    }
                }
            }

            EditorGUILayout.EndVertical();
            
        }

        protected override void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Delete Node"), false, OnClickDeleteNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickDeleteNode()
        {
            Debug.Log("Clicked delete node");
        }
    }
}