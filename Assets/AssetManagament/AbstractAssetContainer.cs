using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetManagament
{
    public abstract class AbstractAssetContainer : ScriptableObject
    {
        [SerializeField] private AssetPoolInject<GameObject> _test;
        
        public abstract Task<TObject> GetAsset<TObject>(string key);
        public abstract void ReadAllKeys(List<(BindKey, Type)> writeList);

        public abstract BindKey GetBindKey(string key);
    }
}