using System;
using System.Collections.Generic;
using System.Reflection;

namespace Injecting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectDependencyAttribute : Attribute
    {
        public readonly string Key;
        public bool IsReleasable;

        public InjectDependencyAttribute(string key = null, bool isReleasable = false)
        {
            Key = string.IsNullOrWhiteSpace(key) ? String.Empty : key;
            IsReleasable = isReleasable;
        }
    }

    public interface IInjectCallbacks
    {
        void Injected();
    }

    public interface IFabric
    {
        object Create();
        void Release(object obj);
    }

    public interface IFabric<T>
    {
        T Create();
        void Release(T tObj);
    }

    public interface IParametricFabric<TParameter>
    {
        object Create(TParameter parameter);
        void Release(TParameter parameter, object obj);
    }

    public interface IParametricFabric<T, TParameter>
    {
        T Create(TParameter parameter);
        void Release(TParameter parameter, T tObj);
    }


    public class DefaultFabric<T> : IFabric<T>, IFabric
    {
        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public void Release(object obj)
        {
            if(obj is IDisposable disposable)
                disposable.Dispose();
        }

        public void Release(T tObj)
        {
            Release((object)tObj);
        }

        object IFabric.Create()
        {
            return Create();
        }
    }

    public class DefaultFabric : IFabric
    {
        private readonly Type _type;

        public DefaultFabric(Type type)
        {
            _type = type;
        }

        public object Create()
        {
            return Activator.CreateInstance(_type);
        }

        public void Release(object obj)
        {
            if(obj is IDisposable disposable)
                disposable.Dispose();
        }
    }

    public struct ResolveParamater
    {
        public readonly string Id;
        public readonly Type ResolveType;
    }
    
    public class Container : IParametricFabric<ResolveParamater>
    {
        private readonly Dictionary<Type, Dictionary<string, IFabric>> _fabrics =
            new Dictionary<Type, Dictionary<string, IFabric>>();

        public object Create(ResolveParamater resolveParamater)
        {
            return GetFabric(resolveParamater.ResolveType, resolveParamater.Id).Create();
        }

        public void Release(ResolveParamater resolveParamater, object obj)
        {
            GetFabric(resolveParamater.ResolveType, resolveParamater.Id).Release(obj);
        }
        
        public T Resolve<T>(T data)
        {
            return (T)Resolve((object)data);
        }

        public object Resolve(object obj)
        {
            var objType = obj.GetType();
            InjectFields(objType, obj);
            InjectProperties(objType, obj);
            if (obj is IInjectCallbacks injectCallbacks)
            {
                injectCallbacks.Injected();
            }
            return obj;
        }

        private void InjectFields(Type type, object obj)
        {
            var fields = type.GetFields((BindingFlags) (1 << 25 - 1));
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.GetCustomAttribute(typeof(InjectDependencyAttribute)) is InjectDependencyAttribute attribute)
                {
                    var key = attribute.Key;
                    var resolveType = fieldInfo.FieldType;
                    fieldInfo.SetValue(obj, Resolve(resolveType, key));
                }
            }
        }

        private void InjectProperties(Type type, object obj)
        {
            var properties = type.GetProperties((BindingFlags) (1 << 25 - 1));
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetCustomAttribute(typeof(InjectDependencyAttribute)) is InjectDependencyAttribute
                    attribute)
                {
                    var key = attribute.Key;
                    var resolveType = propertyInfo.PropertyType;
                    propertyInfo.SetValue(obj, Resolve(resolveType, key));
                }
            }
        }
        
        public T Resolve<T>()
        {
            return Resolve<T>(string.Empty);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, string.Empty);
        }

        public T Resolve<T>(string key)
        {
            return (T) Resolve(typeof(T), key);
        }

        public object Resolve(Type type, string key)
        {
            var result = GetFabric(type, key).Create();
            return Resolve(result);
        }

        public void RegisterFabric<T, TFabric>(TFabric fabric)
            where TFabric : IFabric<T>, IFabric
        {
            RegisterFabric<T, TFabric>(string.Empty, fabric);
        }

        public void RegisterFabric<T, TFabric>(string key, TFabric fabric)
            where TFabric : IFabric<T>, IFabric
        {
            RegisterFabric(typeof(T), String.Empty, fabric);
        }

        public void RegisterFabric(Type type, IFabric fabric)
        {
            RegisterFabric(type, string.Empty, fabric);
        }

        public void RegisterFabric(Type type, string key, IFabric fabric)
        {
            if (!_fabrics.TryGetValue(type, out var concreteFabrics))
            {
                concreteFabrics = _fabrics[type] = new Dictionary<string, IFabric>();
            }
            this.RegisterAsSingle(typeof(IFabric<>).MakeGenericType(type), fabric);

            concreteFabrics[key] = fabric;
        }

        private IFabric GetFabric(Type type, string key)
        {
            if (!_fabrics.TryGetValue(type, out var concreteFabrics))
            {
                concreteFabrics = _fabrics[type] = new Dictionary<string, IFabric>();
            }

            if (!concreteFabrics.TryGetValue(key, out var fabric))
            {
                fabric = concreteFabrics[key] = new DefaultFabric(type);
            }

            return fabric;
        }
    }
}