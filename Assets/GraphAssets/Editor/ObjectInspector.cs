using UnityEditor;
using UnityEngine;

public class ObjectInspector
{
    public Object obj { get; private set; }
    public SerializedObject serializedObject { get; private set; }
    public Editor editor { get; private set; }
    
    public virtual void Initialize(Object obj)
    {
        this.obj = obj;
        serializedObject = new SerializedObject(obj);
        editor = Editor.CreateEditor(obj);
    }

    public virtual void OnInspectorGUI()
    {
        editor.OnInspectorGUI();
    }
}