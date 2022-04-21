using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using ApplicationScripts.Logic.Features.Indexing;
using EcsViewModelPresenting;
using Game.CoreLogic;
using Injecting;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utilities;
using Utilities.GenericPatterns;
using Object = UnityEngine.Object;


namespace Game
{
    public class AppRoot : MonoBehaviour
    {
        [SerializeField] private TextAsset _textAsset;
        private EcsSystems _systems;
        
        private void Awake()
        {
            RegisterContainer();    
        }
        
        private void RegisterContainer()
        {
            var container = Singletone<Container>.instance;
            _systems = new EcsSystems(new EcsWorld());
            _systems.Add(new EcsWorldDebugSystem());
            var importSystem = new ImportSystem(JsonConvert.DeserializeObject<List<JObject>>(_textAsset.text));
            importSystem.Add(new ImportJConvertCommandSystem<MoneyComponent>());
            _systems.Add(importSystem);
            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }

    [Serializable]
    public struct TestIndexComponent : IIndexComponent<string>
    {
        public string Key;
        
        public string GetIndex()
        {
            return Key;
        }
    }
}