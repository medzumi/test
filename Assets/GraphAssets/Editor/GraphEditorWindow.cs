using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagament;
using GraphAssets;
using GraphAssets.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utilities.Unity.Editor;
using Object = UnityEngine.Object;


public class GraphEditorWindow : EditorWindow
{
    private readonly static List<Type> _types = new List<Type>(); 
    
    static GraphEditorWindow()
    {
        _types.AddRange(AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => typeof(ScriptableObject).IsAssignableFrom(t)));
    }
    
    private ScriptableGraph _scriptableGraph;
    private SimpleNode _selectedNode;
    private SimpleGraphView _graphView;
    private IMGUIContainer _inspector;
    private VisualElement _inspectorVisualElement;

    private readonly Dictionary<SimpleNode, ScriptableGraph.ScriptableObjectDescription> _scriptableObjectDescriptions =
        new Dictionary<SimpleNode, ScriptableGraph.ScriptableObjectDescription>();
    private readonly List<SimpleNode> _nodes = new List<SimpleNode>();

    private readonly List<ScriptableGraph.ScriptableObjectDescription> _currentScriptableObjectDescription =
        new List<ScriptableGraph.ScriptableObjectDescription>();

    private readonly Dictionary<Type, Action<GraphElement>> _graphDeleteActions =
        new Dictionary<Type, Action<GraphElement>>();

    private StyleSheet _styleSheet;

    [MenuItem("Window/UI Toolkit/GraphEditorWindow")]
    public static void ShowExample()
    {
        GraphEditorWindow wnd = GetWindow<GraphEditorWindow>();
        wnd.titleContent = new GUIContent("GraphEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GraphAssets/Editor/GraphEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        labelFromUXML.StretchToParentSize();

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        _styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GraphAssets/Editor/GraphEditorWindow.uss");

        _graphView = root.Q<SimpleGraphView>();
        _graphView.OnDropDownMenu = GraphDropDownMenuHandler;
        _graphView.graphViewChanged = GraphViewChanged;
        _graphView.styleSheets.Add(_styleSheet);
        _inspectorVisualElement = root.Q<VisualElement>("left-panel");
        _graphDeleteActions[typeof(SimpleNode)] = GraphDeleteAction;
        _scriptableGraph = null;
        OnSelectionChange();
    }

    private void GraphDeleteAction(GraphElement obj)
    {
        if (obj is SimpleNode simpleNode && _scriptableGraph && _scriptableObjectDescriptions.TryGetValue(simpleNode, out var description))
        {
            _scriptableGraph.DeleteScriptableObjectDescription(description);
        }
    }

    private GraphViewChange GraphViewChanged(GraphViewChange graphviewchange)
    {
        if (graphviewchange.elementsToRemove != null)
        {
            ClearElementsHandler(graphviewchange.elementsToRemove);
        }

        return graphviewchange;
    }

    private void ClearElementsHandler(List<GraphElement> graphviewchangeElementsToRemove)
    {
        foreach (var graphElement in graphviewchangeElementsToRemove)
        {
            if (_graphDeleteActions.TryGetValue(graphElement.GetType(), out var action))
            {
                action?.Invoke(graphElement);
            }
        }
    }

    private void GraphDropDownMenuHandler(DropdownMenu obj)
    {
        if (_scriptableGraph)
        {
            foreach (var type in _types)
            {
                obj.AppendAction(type.FullName.Replace('.', '/'), action =>
                {
                    var scriptableObjectDescription = _scriptableGraph.CreateScriptable(type);
                    AddNode(scriptableObjectDescription);
                    EditorUtility.SetDirty(_scriptableGraph);
                });
            }
        }
    }

    private void AddNode(ScriptableGraph.ScriptableObjectDescription scriptableObjectDescription)
    {
        var node = CreateNode(scriptableObjectDescription);
        _scriptableObjectDescriptions.Add(node, scriptableObjectDescription);
        _graphView.AddElement(node);
        CreatePorts(scriptableObjectDescription, node, new List<Port>());
    }

    private List<Port> CreatePorts(ScriptableGraph.ScriptableObjectDescription objectDescription, Node node, List<Port> ports)
    {
        var serializedObjects = new SerializedObject(objectDescription.scriptableObject);
        var itterator = serializedObjects.GetIterator();
        itterator.Next(true);
        while (itterator.Next(true))
        {
            var t = itterator.GetCurrentPropertyFieldInfo();
            if (typeof(AssetInject).IsAssignableFrom(t.fieldInfo?.FieldType))
            {
                var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    Type.GetType(itterator.FindPropertyRelative("_injectObjectType").stringValue));
                ports.Add(port);
                node.outputContainer.Add(port);
            }
        }

        return ports;
    }
    
    private SimpleNode CreateNode(ScriptableGraph.ScriptableObjectDescription scriptableObjectDescription)
    {
        var node = new SimpleNode();
        node.style.top = scriptableObjectDescription.y;
        node.style.left = scriptableObjectDescription.x;
        node.SetPosition(new Rect(scriptableObjectDescription.x, scriptableObjectDescription.y, 150, 100));
        var oVE = ObjectVisualElement.GetObjectVisualElement(scriptableObjectDescription.scriptableObject
            .GetType());
        oVE.Initialize(scriptableObjectDescription.scriptableObject);
        node.Add(oVE);
        node.OnChangePosition = (v2) =>
        {
            scriptableObjectDescription.x = v2.x;
            scriptableObjectDescription.y = v2.y;
            EditorUtility.SetDirty(_scriptableGraph);
        };
        node.OnSelect = SelectNodeHandler;
        return node;
    }

    private void OnSelectionChange()
    {
        var scriptableGraph = Selection.activeObject as ScriptableGraph;
        if (_scriptableGraph != scriptableGraph)
        {
            Dispose();
            _scriptableGraph = scriptableGraph;
            Initialize(_scriptableGraph);
        }
    }

    private void Dispose()
    {
        foreach (var keyValuePair in _scriptableObjectDescriptions)
        {
            if (keyValuePair.Key != null)
            {
                keyValuePair.Key.OnSelect = null;
                keyValuePair.Key.OnChangePosition = null;
                _graphView.RemoveElement(keyValuePair.Key);
            }
        }
        _scriptableObjectDescriptions.Clear();
    }

    private void SelectNodeHandler(SimpleNode obj)
    {
        if (_selectedNode != obj)
        {
            _selectedNode = obj;
            if (_inspector != null)
            {
                _inspectorVisualElement.Remove(_inspector);
                _inspector.Dispose();
            }

            if (_scriptableObjectDescriptions.TryGetValue(obj, out var scriptableObjectDescription))
            {
                _inspector = new IMGUIContainer(Editor.CreateEditor(scriptableObjectDescription.scriptableObject)
                    .OnInspectorGUI);
                _inspectorVisualElement.Add(_inspector);
            }
        }
    }

    private void Initialize(ScriptableGraph scriptableGraph)
    {
        _currentScriptableObjectDescription.Clear();
        _scriptableObjectDescriptions.Clear();
        foreach (var scriptableObjectDescription in scriptableGraph.ReadScriptableObjects(
            _currentScriptableObjectDescription))
        {
            AddNode(scriptableObjectDescription);
        }
    }
}

public static class NodePortFabric
{
    private static readonly Dictionary<Type, IConcreteNodePortFabric> _fabrics =
        new Dictionary<Type, IConcreteNodePortFabric>();

    [AttributeUsage(AttributeTargets.Class)]
    public class ConcreteNodePortFabricAttribute : Attribute
    {
        public readonly Type Type;

        public ConcreteNodePortFabricAttribute(Type type)
        {
            Type = type;
        }
    }
    
    public interface IConcreteNodePortFabric
    {
        Port CreateForObject(Object obj, Node node);
        Port CreateForProperty(SerializedProperty serializedProperty, Node node);
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ConcretePortFabricAttribute : Attribute
    {
        public readonly Type Type;

        public ConcretePortFabricAttribute(Type type)
        {
            Type = type;
        }
    }
    
    public interface IConcretePortFabric
    {
        Port CreateForObject(Object obj, Node node);
        Port CreateForProperty(SerializedProperty serializedProperty, Node node);
    }
}