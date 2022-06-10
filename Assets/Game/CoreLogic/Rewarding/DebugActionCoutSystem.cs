using ApplicationScripts.Ecs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.CoreLogic.Rewarding
{
    public class DebugActionCoutSystem<TAction, TCount> : EcsSystemBase, IEcsRunSystem
        where TAction : struct
        where TCount : struct, ICount
    {
        private EcsFilter _filter;
        private EcsPool<TCount> _countPool;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _filter = world.Filter<TAction>().Inc<TCount>().End();
            _countPool = world.GetPool<TCount>();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                Debug.Log($"Do {typeof(TAction).Name} {_countPool.Get(entity).Count} times");
            }
        }
    }
}