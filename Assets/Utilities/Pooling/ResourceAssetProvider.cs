using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Pooling
{
    public class ResourceAssetProvider<T> : IConcreteAssetProvider<T>
    {
        private readonly Type _provideType = typeof(T);
        private readonly Dictionary<string, IPool<T>> _pools = new Dictionary<string, IPool<T>>();

        public Type GetProvideType()
        {
            return _provideType;
        }

        public T Get(string key)
        {
            return GetPool(key).Get();
        }

        public void Release(string key, T tObject)
        {
            GetPool(key).Release(tObject);
        }

        public IPool<T> GetPool(string key)
        {
            if (!_pools.TryGetValue(key, out var pool))
            {
                var example = Resources.Load(key);
#if UNITY_EDITOR
                var poolTransform = new GameObject($"Pool Type:{_provideType} Key:{key}").transform;
#endif
                _pools[key] = pool = new Pool<T>(0, () =>
                {
#if UNITY_EDITOR
                    return (T) (object) UnityEngine.Object.Instantiate(example, poolTransform);
#else
                return (T) (object) UnityEngine.Object.Instantiate(example);
#endif
                });
            }

            return pool;
        }
    }
}