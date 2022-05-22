using System;
using Leopotam.EcsLite;

namespace Game.CoreLogic.Rewarding
{
    //ToDo compare system??
    public class ExternalValidationSystem<TValidationComponent, TCounter1, TCounter2> : IEcsPreInitSystem, IEcsRunSystem where TValidationComponent : struct
        where TCounter1 : struct, ICount
        where TCounter2 : struct, ICount
    {
        private EcsPool<TCounter1> _availableRewardComponent;
        private EcsPool<TCounter2> _getRewardCountComponent;
        private EcsPool<TValidationComponent> _validationPool;
        private EcsFilter _filter;
        
        public void PreInit(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _availableRewardComponent = world.GetPool<TCounter1>();
            _getRewardCountComponent = world.GetPool<TCounter2>();
            _validationPool = world.GetPool<TValidationComponent>();
            _filter = world.Filter<TValidationComponent>()
                .Inc<TCounter2>()
                .Inc<TCounter1>()
                .End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                if (_availableRewardComponent.Get(entity).Count < _getRewardCountComponent.Get(entity).Count)
                {
                    _validationPool.Del(entity);
                }
            }
        }
    }
}