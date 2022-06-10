using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.EntityDestroy;
using Game.CoreLogic;
using Game.CoreLogic.AdsConfigurations;
using Game.CoreLogic.Rewarding;
using Leopotam.EcsLite;
using Unity;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif
using UnityEngine;


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
                .Add<AlwaysLinkContainerUpdateSystem<DefaultLink>>();
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
}