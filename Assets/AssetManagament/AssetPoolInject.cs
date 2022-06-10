using System;
using System.Threading.Tasks;
using Utilities.Pooling;
using Utilities.Unity.Extensions;
using Object = UnityEngine.Object;

namespace AssetManagament
{
    [Serializable]
    public class AssetPoolInject<TObject> : AssetInject<TObject> where TObject : Object
    {
        private IPool<TObject> _pool;
        
        public async Task<TObject> CreateAsset()
        {
            if (_pool.IsNullInUnity())
            {
                _pool = (await PoolManager.GetInstanceAsync()).GetOrCreatePool(await GetAsset());
            }

            return _pool.Get();
        }

        public void ReleaseAsset(TObject tObject)
        {
            _pool.Release(tObject);
        }
    }
}