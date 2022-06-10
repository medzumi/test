using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetManagament
{
    [CreateAssetMenu]
    public class SimpleAssetContainer : AbstractAssetContainer
    {
        [SerializeField] private List<Utilities.GenericPatterns.Datas.ValueTuple<BindKey, Object>> _objects = new List<Utilities.GenericPatterns.Datas.ValueTuple<BindKey, Object>>();

        private Dictionary<string, Object> _dictionary = new Dictionary<string, Object>();
        private List<Component> _buffer = new List<Component>();
        
        public override async Task<TObject> GetAsset<TObject>(string key)
        {
            if (!_dictionary.TryGetValue(key, out var obj))
            {
                foreach (var valueTuple in _objects)
                {
                    if (string.Equals(key, valueTuple.Item1.Key))
                    {
                        _dictionary[key] = valueTuple.Item2;
                    }
                }   
            }
            var type = typeof(TObject);
            if (obj is TObject tObj)
            {
                return tObj;
            }
            else if (obj is GameObject gameObject && gameObject.TryGetComponent(out tObj))
            {
                return tObj;
            }
            else if (type == typeof(GameObject) && obj is Component component)
            {
                return (TObject)(object)component.gameObject;
            }

            return default;
        }

        public override void ReadAllKeys(List<(BindKey, Type)> writeList)
        {
            foreach (var valueTuple in _objects)
            {
                writeList.Add((valueTuple.Item1, valueTuple.Item2?.GetType()));
                if (valueTuple.Item2 is GameObject gameObject)
                {
                    _buffer.Clear();
                    gameObject.GetComponents<Component>(_buffer);
                    writeList.Add((valueTuple.Item1, typeof(GameObject)));
                    foreach (var component in _buffer)
                    {
                        writeList.Add((valueTuple.Item1, component.GetType()));
                    }
                }
                else if (valueTuple.Item2 is Component itemComponent)
                {
                    _buffer.Clear();
                    itemComponent.GetComponents<Component>(_buffer);
                    writeList.Add((valueTuple.Item1, typeof(GameObject)));
                    foreach (var component in _buffer)
                    {
                        writeList.Add((valueTuple.Item1, component.GetType()));
                    }
                }
            }
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            _dictionary.Clear();
            foreach (var valueTuple in _objects)
            {
                if (!_dictionary.ContainsKey(valueTuple.Item1.Key))
                {
                    _dictionary[valueTuple.Item1.Key] = valueTuple.Item2;
                }
            }
            #endif
        }

        public override BindKey GetBindKey(string key)
        {
            foreach (var valueTuple in _objects)
            {
                if (valueTuple.Item1.Key == key)
                    return valueTuple.Item1;
            }

            return null;
        }
    }
}