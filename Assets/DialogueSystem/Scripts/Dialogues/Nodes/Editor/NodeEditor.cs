using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.Dialogues.Nodes
{
    public abstract class NodeEditor : Editor
    {
        private bool controlsSafe;
        protected Node node;
        private VisualElement root;

        private void OnEnable()
        {
            node = (Node) target;
            root = new VisualElement();
            controlsSafe = true;

            root.Add(SetupEditorGUI());
            root.Add(SetupBaseNodeEditor());
        }

        public override VisualElement CreateInspectorGUI()
        {
            return root;
        }


        protected abstract VisualElement SetupEditorGUI();

        private VisualElement SetupBaseNodeEditor()
        {
            var elementRoot = new VisualElement();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/DialogueSystem/Scripts/Dialogues/Nodes/Editor/NodeEditor.uss");

            visualTree.CloneTree(elementRoot);
            elementRoot.styleSheets.Add(styleSheet);

            var dangerBox = elementRoot.Q<Box>("boxDanger");

            var dangerBoxIcon = elementRoot.Q<Image>("dangerBoxIcon");
            dangerBoxIcon.image =
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/DialogueSystem/Images/GameIcons/hazard-sign.png");
            dangerBoxIcon.scaleMode = ScaleMode.ScaleToFit;

            var enumNodeTypeField = elementRoot.Q<EnumField>("enumNodeType");
            enumNodeTypeField.Init(node.Type);
            enumNodeTypeField.RegisterCallback<ChangeEvent<Enum>>(evt => node.SetNodeType((NodeType) evt.newValue));

            var v2PositionField = elementRoot.Q<Vector2Field>("v2Position");
            v2PositionField.value = node.Position;
            v2PositionField.RegisterValueChangedCallback(evt => node.SetPosition(evt.newValue));

            var tglSafety = elementRoot.Q<ToolbarToggle>("tglSafety");
            tglSafety.value = !controlsSafe;
            tglSafety.RegisterValueChangedCallback(evt =>
            {
                HandleDangerZoneSafety(evt.newValue, dangerBox, enumNodeTypeField, v2PositionField);
            });

            HandleDangerZoneSafety(controlsSafe, dangerBox, enumNodeTypeField, v2PositionField);
            return elementRoot;
        }

        private void HandleDangerZoneSafety(
            bool safety, Box dangerBox,
            EnumField enumNodeTypeField, Vector2Field v2PositionField)
        {
            controlsSafe = safety;
            if (controlsSafe)
            {
                dangerBox.AddToClassList("safe");
            }
            else
            {
                dangerBox.RemoveFromClassList("safe");
            }

            enumNodeTypeField.SetEnabled(!controlsSafe);
            v2PositionField.SetEnabled(!controlsSafe);
            Repaint();
        }
    }
}