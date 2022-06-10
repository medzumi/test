using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ObjectVisualElement : VisualElement
{
    private readonly static Dictionary<Type, Type> _objectVisualElement =
        new Dictionary<Type, Type>();
    private readonly static Dictionary<Type, Type> _objectInspectors =
        new Dictionary<Type, Type>();

    static ObjectVisualElement()
    {
        var assignableType = typeof(ObjectInspector);
        var linq = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => assignableType.IsAssignableFrom(t)
                        && t.GetCustomAttribute<GraphInspectorAttribute>() != null
                        && !t.IsAbstract && !t.IsGenericType)
            .Select(t => (t, (t.GetCustomAttribute<GraphInspectorAttribute>() as GraphInspectorAttribute).Type));
        foreach (var valueTuple in linq)
        {
            _objectInspectors.Add(valueTuple.Type, valueTuple.t);
        }

        assignableType = typeof(ObjectVisualElement);
        linq = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => assignableType.IsAssignableFrom(t)
                        && t.GetCustomAttribute<GraphInspectorAttribute>() != null
                        && !t.IsAbstract && !t.IsGenericType)
            .Select(t => (t, (t.GetCustomAttribute<GraphInspectorAttribute>() as GraphInspectorAttribute).Type));

        foreach (var valueTuple in linq)
        {
            _objectVisualElement.Add(valueTuple.Type, valueTuple.t);
        }
    }

    public static ObjectVisualElement GetObjectVisualElement(Type type)
    {
        if (_objectVisualElement.TryGetValue(type, out var veType))
        {
            return Activator.CreateInstance(veType) as ObjectVisualElement;
        }
        else
        {
            return new ObjectVisualElement();
        }
    }
    
    public Object obj { get; private set; }

    private ObjectInspector _objectInspector;

    public void Initialize(Object obj)
    {
        this.obj = obj;
        OnEnable();
    }

    protected virtual void OnEnable()
    {
        if (_objectInspectors.TryGetValue(obj.GetType(), out var inspectorType))
        {
            _objectInspector = Activator.CreateInstance(inspectorType) as ObjectInspector;
        }
        else
        {
            _objectInspector = new ObjectInspector();
        } 
        _objectInspector.Initialize(obj);
        Add(new IMGUIContainer(_objectInspector.OnInspectorGUI));
    }
}