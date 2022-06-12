using System;
using System.Collections.Generic;
using AssetManagament;
using UnityEngine;
using Utilities.CodeExtensions;
using Utilities.Unity.PropertyAttributes;

namespace GraphAssets
{
    [CreateAssetMenu]
    public class ScriptableGraph : ScriptableObject
    {
        [SerializeField]
        private AssetInject<ScriptableObject> _test;
        
        [Serializable]
        public class ScriptableObjectDescription
        {
            public float x;
            public float y;
            public ScriptableObject scriptableObject;
        }
        
        [SerializeField] 
        private List<ScriptableObjectDescription> _scriptableObjects = new List<ScriptableObjectDescription>();

        public List<ScriptableObjectDescription> ReadScriptableObjects(List<ScriptableObjectDescription> scriptableObjects)
        {
            scriptableObjects.AddRange(_scriptableObjects);
            return scriptableObjects;
        }

        public ScriptableObjectDescription CreateScriptable(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                var scriptable = CreateInstance(type);
                scriptable.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                var scriptableDescription = new ScriptableObjectDescription()
                {
                    x = _scriptableObjects.Count * 50,
                    y = _scriptableObjects.Count * 50,
                    scriptableObject = scriptable
                };
                _scriptableObjects.Add(scriptableDescription);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.AddObjectToAsset(scriptable, this);
                UnityEditor.EditorUtility.SetDirty(this);
                return scriptableDescription;
#endif
            }
            else
            {
                return null;
            }
        }

        private static readonly List<ScriptableObject> _buffer = new List<ScriptableObject>();
        
        public void DeleteScriptableObjectDescription(ScriptableObjectDescription description)
        {
            _buffer.Clear();
            for (int i = 0; i < _scriptableObjects.Count; i++)
            {
                if (_scriptableObjects[i].scriptableObject == description.scriptableObject)
                {
                    _buffer.Add(description.scriptableObject);
                    _scriptableObjects.RemoveZeroAlloc(_scriptableObjects[i]);
                }
            }
            foreach (var scriptableObject in _buffer)
            {
#if UNITY_EDITOR
                DestroyImmediate(scriptableObject);
                UnityEditor.EditorUtility.SetDirty(this);
#else
                Destroy(scriptableObject);
#endif
            }
        }
    }
}