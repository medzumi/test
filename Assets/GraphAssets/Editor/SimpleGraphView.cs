using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphAssets.Editor
{
    public class SimpleGraphView : GraphView
    {
        public Action<DropdownMenu> OnDropDownMenu;

        public new class UxmlFactory : UxmlFactory<SimpleGraphView, GraphView.UxmlTraits>
        {
            
        }

        public SimpleGraphView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            OnDropDownMenu?.Invoke(evt.menu);
        }
    }
}