using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.Buttons;

namespace GraphAssets
{
    [CreateAssetMenu]
    public class TestScriptable : ScriptableObject
    {
        [SerializeField] private List<TestScriptable> _test = new List<TestScriptable>();

        [Button]
        private void Test()
        {
            var t = CreateInstance<TestScriptable>();
            _test.Add(t);
            t.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            AssetDatabase.AddObjectToAsset(t, this);
            AssetDatabase.SaveAssets();
        }
    }
}