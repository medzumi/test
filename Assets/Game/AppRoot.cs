using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using ApplicationScripts.Logic.Features.Indexing;
using EcsViewModelPresenting;
using Game.CoreLogic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using UnityEngine;
using Utilities;
using Utilities.GenericPatterns;
using ViewModel;
using Object = UnityEngine.Object;


namespace Game
{
    [DefaultExecutionOrder(-1)]
    public class AppRoot : MonoBehaviour
    {
        [SerializeField] private TextAsset _textAsset;
        [SerializeField] private TestSystem _testSystem;
        private EcsSystems _systems;
        
        private void Awake()
        {
            RegisterContainer();    
        }
        
        private void RegisterContainer()
        {
            _systems = new EcsSystems(new EcsWorld());
            _systems.Add(new EcsWorldDebugSystem())
                .Add<ViewModelUpdateSystem<MoneyComponent>>()
                .Add<ViewModelUpdateSystem<InteractComponent>>()
                .Add(_testSystem);
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

    [Serializable]
    public class TestSystem : EcsSystemBase
    {
        [SerializeField] private MonoViewModel _rootViewModel;

        private EcsFilter _filter;
        private EcsPool<MoneyComponent> _moneyPool;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            var entity1 = world.NewEntity();
            var entity2 = world.NewEntity();
            var entity3 = world.NewEntity();
            world.GetPool<MoneyComponent>()
                .Add(entity1, new MoneyComponent()
                {
                    Value = -999
                });
            world.GetPool<MoneyComponent>()
                .Add(entity2, new MoneyComponent()
                {
                    Value = -999999999
                });
            world.GetPool<InteractComponent>()
                .Add(entity3, new InteractComponent()
                {
                    Player = entity1,
                    Trader = entity2
                });
            _filter = world.Filter<MoneyComponent>().End();
            _moneyPool = world.GetPool<MoneyComponent>();
            PresenterSettings.instance.PresenterResolver.Resolve("Test1").Initialize(new EcsPresenterData()
            {
                ModelEntity = entity3,
                ModelWorld = world,
                ViewModel = _rootViewModel
            });
        }

        public override void Run(EcsSystems systems)
        {
            base.Run(systems);
            foreach (var entity in _filter)
            {
                var component = _moneyPool.Get(entity);
                component.Value += 1;
                _moneyPool.Set(entity, component);
            }
        }
    }
}