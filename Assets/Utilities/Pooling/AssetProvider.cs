using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pooling
{
    public class AssetProvider : MonoBehaviour, IAssetProvider
    {
        private readonly Dictionary<Type, IConcreteAssetProvider> _providers =
            new Dictionary<Type, IConcreteAssetProvider>();

        private void Awake()
        {
            foreach (var VARIABLE in GetComponents<IConcreteAssetProvider>())
            {
                _providers[VARIABLE.GetProvideType()] = VARIABLE;
            }
        }

        public T GetAsset<T>(string key)
        {
            return GetConcreteAssetProvider<T>().Get(key);
        }

        public void Release<T>(string key, T tObject)
        {
            GetConcreteAssetProvider<T>().Release(key, tObject);
        }

        public IConcreteAssetProvider<T> GetConcreteAssetProvider<T>()
        {
            if (!_providers.TryGetValue(typeof(T), out var provider))
            {
                _providers[typeof(T)] = provider = new ResourceAssetProvider<T>();
            }

            return (IConcreteAssetProvider<T>)provider;
        }
    }
}