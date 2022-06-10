using System;
using ApplicationScripts.Ecs;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

namespace Game.CoreLogic
{
    public class ConditionedLifeTimeContainerSystem<TLinkComponent> : EcsSystemBase, IEcsRunSystem
        where TLinkComponent : struct, ILinkComponent
    {
        private EcsFilter _createFilter;
        private EcsFilter _clearFilter;
        private EcsFilter _linkFilter;
        private EcsPool<LinkContainer<TLinkComponent>> _linkContainerPool;
        private EcsPool<TLinkComponent> _linkPool;
        private EcsWorld _world;

        public ConditionedLifeTimeContainerSystem(EcsWorld.Mask createMask, EcsWorld.Mask clearMask)
        {
            _createFilter = createMask.Exc<LinkContainer<TLinkComponent>>().End();
            _clearFilter = clearMask.Inc<LinkContainer<TLinkComponent>>().End();
            if (_clearFilter.GetWorld() != _createFilter.GetWorld())
            {
                throw new Exception("Not matched worlds");
            }

            _world = _createFilter.GetWorld();
        }

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            _linkPool = _world.GetPool<TLinkComponent>();
            _linkContainerPool = _world.GetPool<LinkContainer<TLinkComponent>>();
            _linkFilter = _world.Filter<TLinkComponent>().End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _createFilter)
            {
                var container = _linkContainerPool
                    .Add(entity)
                    .Links;
                foreach (var linkedEntity in _linkFilter)
                {
                    if (_linkPool.Get(linkedEntity).GetLink() == entity)
                    {
                        container.AddAsUnique(linkedEntity);
                    }
                }
            }

            foreach (var entity in _clearFilter)
            {
                _linkContainerPool.Del(entity);
            }
        }
    }
}