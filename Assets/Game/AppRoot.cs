using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.EntityDestroy;
using Game.CoreLogic;
using Game.CoreLogic.AdsConfigurations;
using Game.CoreLogic.Rewarding;
using Game.PresenterLogic;
using Leopotam.EcsLite;
using presenting.ecslite;
using Unity;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif
using UnityEngine;
using unityPresenting.Unity;
using ViewModel;


namespace Game
{
    [DefaultExecutionOrder(-1)]
    public class AppRoot : MonoBehaviour
    {
        public TestSystem _testSystem;
        private EcsSystems _systems;
        
        private void Awake()
        {
            RegisterContainer();    
        }
        
        private void RegisterContainer()
        {
            _systems = new EcsSystems(new EcsWorld());
#if UNITY_EDITOR
            _systems.Add(new EcsWorldDebugSystem());
#endif
            _systems.Add(_testSystem);
            _systems
                .Add<AlwaysLinkContainerUpdateSystem<DefaultLink>>()
                .Add<ViewModelUpdateSystem<InteractComponent>>()
                .Add<ViewModelUpdateSystem<LinkContainer<DefaultLink>>>()
                .Add<ViewModelUpdateSystem<NameComponent>>()
                .Add<ViewModelUpdateSystem<UnifiedViewKeyComponent>>()
                .Add<ViewModelUpdateSystem<CategoryComponent>>()
                .Add<ViewModelUpdateSystem<TimerComponent>>();
            var adsFeature = new AdsPurchaseService();
            adsFeature
                .PreAdsSystems
                .Add<ExternalValidationSystem<AdsStartWatchComponent, AdsAvailable, AdsWatched>>();
            adsFeature
                .AlreadyAdsSystems
                .Add<EventTranslatorSystem<AdsSuccessFinish, RewardCommand>>();

            var rewardFeature = new RewardFeature();
            _systems.Add(adsFeature)
                .Add(rewardFeature);

            _systems.Add(new DestroySystem(string.Empty));
            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }

    [Serializable]
    public class TestSystem : EcsSystemBase
    {
        [SerializeField] private MonoViewModel _monoViewModel;

        [PresenterKeyProperty(typeof(EcsPresenterData), typeof(IViewModel))] [SerializeField]
        private string _presenterKey;
        
        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            var world = systems.GetWorld();
            var nameComponent = world.GetPool<NameComponent>();
            var timerComponent = world.GetPool<TimerComponent>();
            var adsComponent = world.GetPool<AdsComponent>();
            var unifiedViewKeyComponent = world.GetPool<UnifiedViewKeyComponent>();
            var categoryComponent = world.GetPool<CategoryComponent>();
            var linkPool = world.GetPool<DefaultLink>();
            var rewardPool = world.GetPool<Reward>();

            var entity = world.NewEntity();
            var shopEntity = entity;
            var modelEntity = entity;

            entity = world.NewEntity();
            var categoryEntity = entity;
            linkPool.Add(entity).SetLink(shopEntity);
            categoryComponent.Add(entity) = new CategoryComponent()
            {
                Value = "Best"
            };
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Best"
            };
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(categoryEntity);
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Lot_1"
            };
            timerComponent.Add(entity) = new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            };
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test1";
            
            entity = world.NewEntity();
            var purchaseOptionParentEntity = entity;
            linkPool.Add(entity).SetLink(categoryEntity);
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Lot_2"
            };
            timerComponent.Add(entity) = new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            };
            unifiedViewKeyComponent.Add(entity) = new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            };

            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x1";
            rewardPool.Add(entity).count = 1;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x5";
            rewardPool.Add(entity).count = 5;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x10";
            rewardPool.Add(entity).count = 10;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";

            PresenterSettings.instance.PresenterResolver.Resolve<EcsPresenterData, IViewModel>(_presenterKey)
                .Initialize(new EcsPresenterData()
                {
                    ModelEntity = modelEntity,
                    ModelWorld = world,
                }, _monoViewModel);
        }
    }
}