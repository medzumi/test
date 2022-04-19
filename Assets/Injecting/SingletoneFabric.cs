using System;
using UnityEngine;

namespace Injecting
{
    public class SingletoneFabric<T> : IFabric<T>, IFabric
    {
        private readonly T _instance;

        public SingletoneFabric(T instance)
        {
            _instance = instance;
        }

        public T Create()
        {
            return _instance;
        }

        public void Release(object obj)
        {
            
        }

        public void Release(T tObj)
        {
            
        }

        object IFabric.Create()
        {
            return Create();
        }
    }

    public class SingletoneFabric : IFabric
    {
        private readonly object _instance;

        public SingletoneFabric(object instance)
        {
            _instance = instance;
        }

        public object Create()
        {
            return _instance;
        }

        public void Release(object obj)
        {
            
        }
    }

    public static class InjectExtensions
    {
        public static Container RegisterAsSingle<T>(this Container container, string key, T instance)
        {
            container.RegisterFabric<T, SingletoneFabric<T>>(key, new SingletoneFabric<T>(instance));
            return container;
        }

        public static Container RegisterAsSingle<T>(this Container container, T instance)
        {
            container.RegisterFabric<T, SingletoneFabric<T>>(new SingletoneFabric<T>(instance));
            return container;
        }

        public static Container RegisterAsSingle(this Container container, Type type, object instance)
        {
            return container.RegisterAsSingle(type, string.Empty, instance);
        }
        
        public static Container RegisterAsSingle(this Container container, Type type, string key, object instance)
        {
            container.RegisterFabric(type, key, new SingletoneFabric(instance));
            return container;
        }
    }
}