using lastmilegames.DialogueSystem.Nodes;
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

            AddElement(GenerateEntryPointNode());
        }

        private BaseNode GenerateEntryPointNode()
        {
            BaseNode node = new BaseNode()
            {
                title = "Start",
            };
            Debug.Log($"Starting Point GUID: {node.GUID}");

            node.GeneratePort("Next", Direction.Output);
            node.capabilities &= ~Capabilities.Deletable; // Node cannot be deleted

            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }
    }
}