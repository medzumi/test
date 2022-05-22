using ApplicationScripts.Ecs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.CoreLogic
{
    public class AdsPurchaseService : EcsSystemBase, IEcsRunSystem
    {
        private EcsPool<AdsComponent> _adsPool;
        private EcsPool<AdsStartWatchComponent> _pool2;
        private EcsPool<AdsSuccessFinish> _pool3;

        private EcsFilter _filter;
        private EcsFilter _filter2;

        public SystemCollection PreAdsSystems { get; private set; } = new SystemCollection();
        public SystemCollection AlreadyAdsSystems { get; private set; } = new SystemCollection();

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _adsPool = world.GetPool<AdsComponent>();
            _pool2 = world.GetPool<AdsStartWatchComponent>();
            _pool3 = world.GetPool<AdsSuccessFinish>();
            _filter = world.Filter<AdsComponent>()
                .Inc<AdsStartWatchComponent>()
                .End();
            _filter2 = world.Filter<AdsSuccessFinish>().End();
            PreAdsSystems.PreInit(systems);
            AlreadyAdsSystems.PreInit(systems);
        }

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            PreAdsSystems.Init(systems);
            AlreadyAdsSystems.Init(systems);
        }

        public override void Destroy(EcsSystems systems)
        {
            base.Destroy(systems);
            PreAdsSystems.Destroy(systems);
            AlreadyAdsSystems.Destroy(systems);
        }

        public override void PostDestroy(EcsSystems systems)
        {
            base.PostDestroy(systems);
            PreAdsSystems.PostDestroy(systems);
            AlreadyAdsSystems.PostDestroy(systems);
        }

        public void Run(EcsSystems systems)
        {
            PreAdsSystems.Run(systems);
            foreach (var variable in _filter)
            {
                _pool2.Del(variable);
                Debug.Log($"Ads watch {_adsPool.Get(variable).AdsPlacement}");
                _pool3.Add(variable) = new AdsSuccessFinish();
            }
            AlreadyAdsSystems.Run(systems);
            foreach (var entity in _filter2)
            {
                _pool3.Del(entity);
            }
        }
    }
}