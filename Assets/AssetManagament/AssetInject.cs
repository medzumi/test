using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Unity.PropertyAttributes;
using Object = UnityEngine.Object;

namespace AssetManagament
{
    [Serializable]
    public class AssetInject : ISerializationCallbackReceiver
    {
        [SerializeField] protected Object _tObject;
        [SerializeField] protected string _key;
        [SerializeField] protected InjectType _injectType;
        [SerializeField] [ReadOnlyField] protected string _injectObjectType;
        
        public virtual void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(_injectObjectType))
            {
                _injectObjectType = typeof(Object).AssemblyQualifiedName;
            }
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
    
    [Serializable]
    public class AssetInject<TObject> : AssetInject where TObject : Object
    {
        public async Task<TObject> GetAsset()
        {
            if (_injectType == InjectType.WithObject)
            {
                (await AssetManager.GetInstanceAsync()).Inject(_tObject, _key);
                return _tObject as TObject;
            }
            else
            {
                return await (await AssetManager.GetInstanceAsync()).GetAsset<TObject>(_key);
            }
        }

        public override void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(_injectObjectType) || _injectObjectType != typeof(TObject).AssemblyQualifiedName)
            {
                _injectObjectType = typeof(TObject).AssemblyQualifiedName;
            }
            base.OnBeforeSerialize();
        }
    }
}