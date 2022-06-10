using System;

[AttributeUsage(AttributeTargets.Class)]
public class GraphInspectorAttribute : Attribute
{
    public readonly Type Type;

    public GraphInspectorAttribute(Type type)
    {
        Type = type;
    }
}