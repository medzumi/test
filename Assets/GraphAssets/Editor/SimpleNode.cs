using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphAssets.Editor
{
    public class SimpleNode : Node
    {
        public new class UxmlFactory : UxmlFactory<SimpleNode, Node.UxmlTraits>
        {
            
        }

        public Action<Vector2> OnChangePosition;
        public Action<SimpleNode> OnSelect;

        public override void OnSelected()
        {
            base.OnSelected();
            OnSelect?.Invoke(this);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            OnChangePosition?.Invoke(new Vector2(newPos.x, newPos.y));
        }
    }
}