using System;
using System.Linq;
using lastmilegames.DialogueSystem.NodeData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem.DialogueGraphEditor
{
    public class BlackboardProvider
    {
        public Blackboard Blackboard { get; private set; }

        private readonly DialogueGraphView graphView;

        public BlackboardProvider(DialogueGraphView graphView)
        {
            this.graphView = graphView;

            Blackboard = new Blackboard
            {
                scrollable = true,
                addItemRequested = AddItemRequested,
                editTextRequested = EditTextRequested
            };
        }

        public void RefreshProperties()
        {
            Blackboard.contentContainer.Clear();
            foreach (ExposedProperty property in graphView.exposedProperties)
            {
                Blackboard.Add(GenerateBlackboardField(property));
            }
        }

        private void EditTextRequested(Blackboard blackboard, VisualElement element, string value)
        {
            string currentName = ((BlackboardField) element).text;
            string nextName = ValidateName(value);

            if (graphView.exposedProperties.Any(property => property.propertyName == nextName))
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    $"A property with the name \"{nextName}\" already exists.",
                    "OK"
                );
                return;
            }

            int propertyIndex = graphView.exposedProperties.FindIndex(x => x.propertyName == currentName);
            graphView.exposedProperties[propertyIndex].propertyName = nextName;
            ((BlackboardField) element).text = nextName;
        }

        private void AddItemRequested(Blackboard blackboard)
        {
            var property = new ExposedProperty();
            property.propertyName = ValidateName(property.propertyName);
            graphView.exposedProperties.Add(property);
            
            blackboard.Add(GenerateBlackboardField(property));
        }

        private VisualElement GenerateBlackboardField(ExposedProperty property)
        {
            var container = new VisualElement();
            var blackboardField = new BlackboardField
            {
                text = property.propertyName,
                typeText = "String",
            };
            container.Add(blackboardField);

            var propertyValueTextField = new TextField("Value:");
            propertyValueTextField.Q(className: "unity-label").style.minWidth = 50;
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                int changingPropertyIndex = graphView.exposedProperties.FindIndex(
                    exposedProperty => exposedProperty.propertyName == property.propertyName);
                graphView.exposedProperties[changingPropertyIndex].propertyValue = evt.newValue;
            });

            var blackboardValueRow = new BlackboardRow(propertyValueTextField, propertyValueTextField);
            container.Add(blackboardValueRow);

            return container;
        }

        private string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var newName = "";
            string[] nameArray = name.Split(new[] {"@@"}, StringSplitOptions.None);

            foreach (string s in nameArray)
            {
                if (s.Trim() == "@@") continue;

                newName += s.Trim().Normalize().ToUpper();
            }

            var nextName = $@"@@{newName}@@";

            int matchCount = graphView.exposedProperties.FindAll(property => property.propertyName == nextName).Count;

            if (matchCount > 0)
            {
                return $@"@@{newName}{matchCount}@@";
            }
            else
            {
                return nextName;
            }
        }
    }
}