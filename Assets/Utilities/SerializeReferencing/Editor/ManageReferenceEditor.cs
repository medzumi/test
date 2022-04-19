using UnityEditor;
using UnityEngine;

namespace Utilities.SerializeReferencing.Editor
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class ManageReferenceEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                var guiContent = string.IsNullOrWhiteSpace(property.managedReferenceFullTypename)
                    ? property.managedReferenceFieldTypename
                    : property.managedReferenceFullTypename;
                EditorGUI.PropertyField(position, property, new GUIContent(guiContent), true);
            }

            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}