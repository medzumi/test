using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.EntityDestroy;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

namespace Game.CoreLogic
{
    public class AlwaysLinkContainerUpdateSystem<TLinkComponent> : EcsSystemBase, IEcsRunSystem where TLinkComponent : struct, ILinkComponent
    {
        private EcsPool<TLinkComponent> _linkComponentPool;
        private EcsPool<LinkContainer<TLinkComponent>> _linkContainerPool;
        private EcsPool<RequestChangeLink<TLinkComponent>> _requestPool;

        private EcsFilter _toDestroyEntitiesFilter;
        private EcsFilter _changeLinkFilter;
        private EcsFilter _linkedEntitiesFilter;
        private readonly List<int> _buffer = new List<int>();

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _linkComponentPool = world.GetPool<TLinkComponent>();
            _linkContainerPool = world.GetPool<LinkContainer<TLinkComponent>>();
            _requestPool = world.GetPool<RequestChangeLink<TLinkComponent>>();

            _linkedEntitiesFilter = world.Filter<TLinkComponent>().End();
            _changeLinkFilter = world.Filter<RequestChangeLink<TLinkComponent>>().End();
            _toDestroyEntitiesFilter = world.Filter<DestroyComponent>().Inc<TLinkComponent>().End();
        }

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            foreach (var entity in _linkedEntitiesFilter)
            {
                var index = _linkComponentPool
                    .Get(entity)
                    .GetLink();
                _linkContainerPool
                    .EnsureGet(index)
                    .Links
                    .AddAsUnique(entity);
            }
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _changeLinkFilter)
            {
                var requestComponent = _requestPool.Get(entity);
                var isHasLink = _linkComponentPool.Has(entity);
                if (isHasLink)
                {
                    var index = _linkComponentPool
                        .Get(entity)
                        .GetLink();

                    _linkContainerPool
                        .Get(index)
                        .Links
                        .RemoveZeroAlloc(entity);
                    _linkComponentPool
                        .Get(entity)
                        .SetLink(requestComponent.LinkToEntity);
                }
                else
                {
                    _linkComponentPool
                        .Add(entity).SetLink(requestComponent.LinkToEntity);
                }

                _linkContainerPool.EnsureGet(entity).Links.AddAsUnique(entity);
            }

            foreach (var entity in _toDestroyEntitiesFilter)
            {
                var index = _linkComponentPool
                    .Get(entity)
                    .GetLink();
                _linkContainerPool
                    .EnsureGet(index)
                    .Links
                    .RemoveZeroAlloc(entity);
            }
        }
    }
}