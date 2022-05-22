using ApplicationScripts.Ecs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.CoreLogic.Rewarding
{
    public class RewardFeature : SystemCollection
    {
        private EcsPool<RewardCommand> _pool;
        private EcsPool<Reward> _pool2;
        private EcsFilter _filter;
        private EcsFilter _filter2;

        public RewardFeature()
        {
#if UNITY_EDITOR
            Add(new DebugActionCoutSystem<RewardCommand, Reward>());
#endif
        }
        
        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _pool = world.GetPool<RewardCommand>();
            _pool2 = world.GetPool<Reward>();
            _filter = world.Filter<RewardCommand>().End();
            _filter2 = world.Filter<RewardCommand>().Exc<Reward>().End();
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _filter2)
            {
                _pool.Del(entity);
            }
            base.Run(systems);
            foreach (var entity in _filter)
            {
                _pool.Del(entity);
            }
        }
    }
}