using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pooling;
using Utilities.Unity.ScriptableSingletone;
using Object = UnityEngine.Object;

namespace AssetManagament
{
    public class PoolManager : RuntimeScriptableSingletone<PoolManager>
    {
        private readonly Dictionary<object, object> _poolDictionary = new Dictionary<object, object>();

        public IPool<TObject> GetOrCreatePool<TObject>(TObject originObject)
        {
            if (originObject is Component component)
            {
                return GetOrCreateMonoPool<TObject>(component);
            }

            if (originObject is GameObject gameObject)
            {
                return GetOrCreateMonoPool(gameObject) as IPool<TObject>;
            }

            return null;
        }

        public IPool<TObject> GetOrCreateMonoPool<TObject>(TObject monoOriginObject) where TObject : Component
        {
            if (_poolDictionary.TryGetValue(monoOriginObject, out var pool))
            {
                var root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}")).transform;
                DontDestroyOnLoad(root.gameObject);
                _poolDictionary[monoOriginObject] = pool = new Pool<TObject>(0, () =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }

                        return Instantiate(monoOriginObject, root);
                    },
                    null,
                    (tObj) =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }

                        tObj.transform.SetParent(root);
                        tObj.gameObject.SetActive(false);
                    });
            }

            return pool as IPool<TObject>;
        }
        
        private IPool<TObject> GetOrCreateMonoPool<TObject>(Component monoOriginObject)
        {
            if (_poolDictionary.TryGetValue(monoOriginObject, out var pool))
            {
                var root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}")).transform;
                DontDestroyOnLoad(root.gameObject);
                _poolDictionary[monoOriginObject] = pool = new Pool<TObject>(0, () =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }

                        return (TObject)(object)Instantiate(monoOriginObject, root);
                    },
                    null,
                    (tObj) =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(TObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }

                        var component = tObj as Component;
                        component.transform.SetParent(root);
                        component.gameObject.SetActive(false);
                    });
            }

            return pool as IPool<TObject>;
        }
        
        private IPool<GameObject> GetOrCreateMonoPool(GameObject monoOriginObject)
        {
            if (_poolDictionary.TryGetValue(monoOriginObject, out var pool))
            {
                var root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(GameObject).Name}")).transform;
                DontDestroyOnLoad(root.gameObject);
                _poolDictionary[monoOriginObject] = pool = new Pool<GameObject>(0, () =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(GameObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }

                        return Instantiate(monoOriginObject, root);
                    },
                    null,
                    (tObj) =>
                    {
                        if (!root)
                        {
                            root = (new GameObject($"Pool of {monoOriginObject.name} as {typeof(GameObject).Name}"))
                                .transform;
                            DontDestroyOnLoad(root.gameObject);
                        }
                        
                        tObj.transform.SetParent(root);
                        tObj.SetActive(false);
                    });
            }

            return pool as IPool<GameObject>;
        }
    }
}