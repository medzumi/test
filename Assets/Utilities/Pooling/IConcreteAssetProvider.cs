using System;

namespace Utilities.Pooling
{
    public interface IConcreteAssetProvider
    {
        Type GetProvideType();
    }

    public interface IConcreteAssetProvider<T> : IConcreteAssetProvider
    {
        T Get(string key);
        
        void Release(string key, T obj);
    }
}