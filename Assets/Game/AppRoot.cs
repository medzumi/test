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
            _systems.Add(new IndexedEntityLibrarySystem<TestIndexComponent, string>())
                .Add(new IndexedEntityLibrarySystem<ImportableComponent, string>());
            var importSystem = new ImportSystem(JsonConvert.DeserializeObject<List<JObject>>(_textAsset.text));
            importSystem.Add(new ImportJConvertCommandSystem<MoneyComponent>());
            _systems.Add(importSystem);
            _systems.Add(new TestSystem());
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

    public class TestSystem : EcsSystemBase
    {
        private EcsPool<ReferenceComponent<GameObject>> _gameObjectPool;
        private EcsPool<ImportCommand> _importCmd;
        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsPool<MoneyPresenter> _moneyBindData;

        private int _money = -9999;

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            var world = _world = systems.GetWorld();
            _gameObjectPool = world.GetPool<ReferenceComponent<GameObject>>();
            _importCmd = world.GetPool<ImportCommand>();
            _moneyBindData = world.GetPool<MoneyPresenter>();
            _filter = world.Filter<MoneyPresenter>().End();
            _importCmd.Add(_world.NewEntity())
                = new ImportCommand()
                {
                    From = new List<string>(){ "Test" }
                };
        }

        public override void Run(EcsSystems systems)
        {
            _money += 1;
            foreach (var entity in _filter)
            {
                _moneyBindData.Read(entity)
                    .Update(new MoneyComponent()
                    {
                        Value = _money
                    });
            }
        }
    }
}