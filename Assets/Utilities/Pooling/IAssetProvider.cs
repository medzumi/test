namespace Utilities.Pooling
{
    public interface IAssetProvider
    {
        T GetAsset<T>(string key);

        void Release<T>(string key, T tObject);

        IConcreteAssetProvider<T> GetConcreteAssetProvider<T>();
    }
}