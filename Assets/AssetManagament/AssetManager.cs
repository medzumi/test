using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Unity.ScriptableSingletone;
using Object = UnityEngine.Object;

namespace AssetManagament
{
    [CreateAssetMenu]
    public class AssetManager : RuntimeScriptableSingletone<AssetManager>
    {
        [SerializeField]
        private List<AbstractAssetContainer> _abstractAssetContainers = new List<AbstractAssetContainer>();
        [SerializeField] private List<AssetInjector> _assetInjectors = new List<AssetInjector>();

        private readonly Dictionary<string, Dictionary<Type, object>> _dictionary =
            new Dictionary<string, Dictionary<Type, object>>();
        
        public void Inject(object obj, string key)
        {
            foreach (var assetInjector in _assetInjectors)
            {
                assetInjector.Inject(obj, key);
            }   
        }

        public async Task<TObject> GetAsset<TObject>(string key) where TObject : Object
        {

            foreach (var abstractAssetContainer in _abstractAssetContainers)
            {
                var asset = await abstractAssetContainer.GetAsset<TObject>(key);
                if (asset)
                    return asset;
            }

            return null;
        }

        public List<(BindKey, Type)> ReadAllKeys(List<(BindKey, Type)> writeList)
        {
            foreach (var abstractAssetContainer in _abstractAssetContainers)
            {
                abstractAssetContainer.ReadAllKeys(writeList);
            }

            return writeList;
        }

        public BindKey GetBindKey(string key)
        {
            foreach (var abstractAssetContainer in _abstractAssetContainers)
            {
                var result = abstractAssetContainer.GetBindKey(key);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private Dictionary<Type, object> GetTypeDictionary(string key)
        {
            if (!_dictionary.TryGetValue(key, out var dictionary))
            {
                _dictionary[key] = dictionary = new Dictionary<Type, object>();
            }

            return dictionary;
        }

        private bool TryGetAsset<TObject>(string key, out TObject obj)
        {
            var dictionary = GetTypeDictionary(key);
            if (dictionary.TryGetValue(typeof(TObject), out var result))
            {
                obj = (TObject)result;
                return true;
            }

            obj = default;
            return false;
        }
    }
}