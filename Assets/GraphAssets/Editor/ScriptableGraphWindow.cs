using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EventSystems;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.CodeExtensions;

namespace GraphAssets.Editor
{
    public class ScriptableGraphWindow : EditorWindow
    {
        private static ScriptableGraphWindow _scriptableGraphWindow;
        
        private ScriptableGraph _scriptableGraph;
        private ScriptableGraphView _graphView;

        [MenuItem("Medzumi/TestWindow")]
        private static void CreateWindow()
        {
            GetWindow<ScriptableGraphWindow>();
        }

        public void CreateGUI()
        {
            _graphView = new ScriptableGraphView();
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void OnSelectionChange()
        {
            var selectedObject = Selection.activeObject as ScriptableGraph;
            if (selectedObject != _scriptableGraph)
            {
                _graphView.Initialize(selectedObject);
                _scriptableGraph = selectedObject;
            }
        }
    }

    public class ScriptableGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ScriptableGraphView, GraphView.UxmlTraits>
        {
            
        }
        
        private ScriptableGraph _scriptableGraph;
        private readonly List<ScriptableGraph.ScriptableObjectDescription> _buffer = new List<ScriptableGraph.ScriptableObjectDescription>();
        private readonly List<ScriptableNode> _scriptableNodes = new List<ScriptableNode>();
        private readonly static List<Type> _createTypes = new List<Type>();

        static ScriptableGraphView()
        {
            _createTypes.AddRange(AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(ScriptableObject).IsAssignableFrom(t)));
        }
        
        public ScriptableGraphView()
        {
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GraphAssets/Editor/GraphEditorWindow.uss");
            styleSheets.Add(styleSheet);
            
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            foreach (var type in _createTypes)
            {
                evt.menu.AppendAction(type.FullName.Replace(".", "/"), action =>
                {
                    if (_scriptableGraph)
                    {
                        var scriptable = _scriptableGraph.CreateScriptable(type);
                        if (scriptable.IsNotNull())
                        {
                            var scriptableView = new BaseScriptableNode();
                            scriptableView.Initialize(scriptable);
                            scriptableView.SetPosition(new Rect(0, 0 ,150, 100));
                            _scriptableNodes.Add(scriptableView);
                            AddElement(scriptableView);
                        }
                    }
                });
            }
        }

        public void Initialize(ScriptableGraph scriptableGraph)
        {
            foreach (var scriptableNode in _scriptableNodes)
            {
                RemoveElement(scriptableNode);
            }
            _scriptableNodes.Clear();
            _buffer.Clear();
            if (_scriptableGraph = scriptableGraph)
            {
                foreach (var scriptableObject in _scriptableGraph.ReadScriptableObjects(_buffer))
                {
                    var node = new BaseScriptableNode();
                    node.Initialize(scriptableObject);
                    AddElement(node);
                    _scriptableNodes.Add(node);
                }
            }
        }
    }

    public abstract class ScriptableNode : Node
    {
        public ScriptableGraph.ScriptableObjectDescription scriptableObjectDescription;
        public virtual void Initialize(ScriptableGraph.ScriptableObjectDescription scriptableObject)
        {
            scriptableObjectDescription = scriptableObject;
            style.left = scriptableObject.x;
            style.top = scriptableObject.y;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            scriptableObjectDescription.x = newPos.x;
            scriptableObjectDescription.y = newPos.y;
        }
    }

    public class BaseScriptableNode : ScriptableNode
    {
        public override void Initialize(ScriptableGraph.ScriptableObjectDescription scriptableObject)
        {
            base.Initialize(scriptableObject);
            IMGUIContainer container =
                new IMGUIContainer(UnityEditor.Editor.CreateEditor(scriptableObject.scriptableObject).OnInspectorGUI);
            Add(container);
        }
    }
}