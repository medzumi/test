using ApplicationScripts.Ecs;
using Game.CoreLogic.Rewarding;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.CoreLogic
{
    public class PurchaseFeature : SystemCollection
    {
        private EcsFilter _filter;
        private EcsFilter _filter2;
        private EcsPool<SuccessPurchase> _pool;
        private EcsPool<CallPurchaseComponent> _pool2;
        private EcsPool<RealValuePriceComponent> _pool3;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _filter = world.Filter<CallPurchaseComponent>().Inc<RealValuePriceComponent>().End();
            _filter2 = world.Filter<SuccessPurchase>().End();
            _pool = world.GetPool<SuccessPurchase>();
            _pool2 = world.GetPool<CallPurchaseComponent>();
            _pool3 = world.GetPool<RealValuePriceComponent>();
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                var call = _pool3.Get(entity);
                _pool2.Del(entity);
                Debug.Log($"Call purchase with bundle {call.Bundle}");
                _pool.Add(entity);
            }
            base.Run(systems);
            foreach (var entity in _filter2)
            {
                _pool.Del(entity);
            }
        }
    }
}