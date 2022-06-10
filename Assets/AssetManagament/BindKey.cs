using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Unity.PropertyAttributes;

namespace AssetManagament
{
    [Serializable]
    public class BindKey : ISerializationCallbackReceiver
    {
        #if UNITY_EDITOR
        private static Dictionary<string, BindKey> _bindKeys = new Dictionary<string,BindKey>();
        private static Dictionary<BindKey, string> _keys = new Dictionary<BindKey, string>();

        private static List<BindKey> _toResetInDictionary = new List<BindKey>();
        private static List<BindKey> _collisions = new List<BindKey>();
        #endif
        
        [SerializeField] private string _name;
        [SerializeField] [ReadOnlyField] private string _key;
        public string Key => _key;
        public string Name => _name;

        public BindKey()
        {
            if (string.IsNullOrWhiteSpace(_key))
            {
                _key = Guid.NewGuid().ToString();
            }
        }

        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
            var currentKey = _key;
            var currentBindKey = this;
            _toResetInDictionary.Clear();
            _collisions.Clear();
            while (true)
            {
                if (_bindKeys.TryGetValue(currentKey, out var bindKey))
                {
                    if (ReferenceEquals(bindKey, currentBindKey))
                    {
                        break;
                    }
                    else
                    {
                        if (string.Equals(bindKey._key, _key))
                        {
                            _toResetInDictionary.Add(bindKey);
                            _collisions.Add(currentBindKey);
                            break;
                        }
                        
                        _toResetInDictionary.Add(currentBindKey);
                        _toResetInDictionary.Add(bindKey);
                        currentKey = bindKey._key;
                        currentBindKey = bindKey;
                        if (!_keys.TryGetValue(bindKey, out var key) || string.Equals(key, _key))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    _bindKeys[_key] = this;
                    break;
                }
            }

            foreach (var bindKey in _toResetInDictionary)
            {
                _bindKeys[bindKey._key] = bindKey;
                _keys[bindKey] = bindKey._key;
            }

            foreach (var bindKey in _collisions)
            {
                bindKey._key = Guid.NewGuid().ToString();
                _bindKeys[bindKey._key] = bindKey;
                _keys[bindKey] = bindKey._key;
            }
            
            if (string.IsNullOrWhiteSpace(_key))
            {
                _key = Guid.NewGuid().ToString();
                _bindKeys[_key] = this;
                _keys[this] = _key;
            }
            #endif
        }

        public void OnAfterDeserialize()
        {
            #if UNITY_EDITOR
            _bindKeys.Clear();
            _collisions.Clear();
            _keys.Clear();
            _toResetInDictionary.Clear();
            #endif
        }
    }

    [SerializeField]
    public class KeyValueCollection<TObject>
    {
        [SerializeField]
        private List<Utilities.GenericPatterns.Datas.ValueTuple<BindKey, TObject>> _collection =
            new List<Utilities.GenericPatterns.Datas.ValueTuple<BindKey, TObject>>();

        public List<Utilities.GenericPatterns.Datas.ValueTuple<BindKey, TObject>> Collection => _collection;
    }
}