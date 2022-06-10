using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.Editor;
using Object = UnityEngine.Object;

namespace AssetManagament.Editor
{
    [CustomPropertyDrawer(typeof(AssetInject), true)]
    public class AssetInjectPropertyDrawer : PropertyDrawer
    {
        private const string TOBJECT = "_tObject";
        private const string KEY = "_key";
        private const string INJECT_TYPE = "_injectType";
        private const string INJECT_OBJECT_TYPE = "_injectObjectType";
        private readonly List<(BindKey, Type)> _buffer = new List<(BindKey, Type)>();

        private readonly string[] _strings = new string[0];
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label)
                   + EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyObj = property.FindPropertyRelative(TOBJECT);
            var propertyKey = property.FindPropertyRelative(KEY);
            var propertyType = property.FindPropertyRelative(INJECT_TYPE);
            var propertyObjType = property.FindPropertyRelative(INJECT_OBJECT_TYPE);

            position = position.DrawPropertyField(propertyType);
            if ((InjectType) propertyType.enumValueIndex == InjectType.WithKey)
            {
                position.DrawPrefixLabel(propertyKey.displayName, out var nextRect)
                    .DrawButton(AssetManager.GetInstance().GetBindKey(propertyKey.stringValue)?.Name, out var result);
                if (result)
                {
                    var genericMenu = new GenericMenu();
                    _buffer.Clear();
                    AssetManager.GetInstance().ReadAllKeys(_buffer);
                    var findType = Type.GetType(propertyObjType.stringValue);
                    foreach (var valueTuple in _buffer)
                    {
                        if (findType.IsAssignableFrom(valueTuple.Item2))
                        {
                            genericMenu.AddItem(new GUIContent(valueTuple.Item1.Name), false, () =>
                            {
                                propertyKey.stringValue = valueTuple.Item1.Key;
                                Debug.Log(valueTuple.Item1.Key, AssetManager.GetInstance().GetAsset<Object>(valueTuple.Item1.Key).Result);
                                propertyKey.serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }
                    genericMenu.ShowAsContext();
                }
            }
            else
            {
                position.DrawObjectReferenceField(propertyObj, Type.GetType(propertyObjType.stringValue));
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}