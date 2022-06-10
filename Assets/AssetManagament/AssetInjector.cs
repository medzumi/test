using UnityEngine;

namespace AssetManagament
{
    public abstract class AssetInjector : ScriptableObject
    {
        public abstract void Inject(object obj, string key);
    }
}