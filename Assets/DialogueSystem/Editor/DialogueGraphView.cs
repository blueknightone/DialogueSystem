using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace lastmilegames.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        /// <summary>
        /// Creates a new instance of DialogueGraphView.
        /// </summary>
        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            // TODO: AddElement(GenerateEntryPointNode());
        }

        private GraphElement GenerateEntryPointNode()
        {
            throw new System.NotImplementedException();
        }
    }
}