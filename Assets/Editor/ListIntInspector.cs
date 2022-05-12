using System;
using System.Collections.Generic;
using Game.CoreLogic;
using Leopotam.EcsLite.UnityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class ContainerComponentInspector : EcsComponentInspectorTyped<ContainerComponent>
    {
        private class Inspector
        {
            private readonly ReorderableList _reorderableList;
            public readonly List<int> _list;
            public bool IsDirty { get; private set; } = false;

            public Inspector(List<int> list)
            {
                _list = list;
                _reorderableList = new ReorderableList(_list, typeof(int));
                _reorderableList.elementHeightCallback = index =>
                {
                    return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, GUIContent.none);
                };
                _reorderableList.drawElementCallback = (rect, index, active, focused) =>
                {
                    var previousElement = _list[index];
                    _list[index] = EditorGUI.IntField(rect, _list[index]);
                    if (previousElement != _list[index])
                        IsDirty = true;
                };
                _reorderableList.onChangedCallback = reorderableList =>
                {
                    IsDirty = true;
                };
            }

            public bool OnGui()
            {
                _reorderableList.DoLayoutList();
                var isDirty = IsDirty;
                IsDirty = false;
                return isDirty;
            }
        }
        
        private readonly Dictionary<int, Inspector> _reorderableLists = new Dictionary<int, Inspector>();

        public override bool OnGuiTyped(string label, ref ContainerComponent value, EcsEntityDebugView entityView)
        {
            if(!_reorderableLists.TryGetValue(entityView.Entity, out var inspector) || !ReferenceEquals(inspector._list, value.List))
            {
                _reorderableLists[entityView.Entity] = inspector = new Inspector(value.List);
            }

            return inspector.OnGui();
        }
    }
}