using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.EntityDestroy;
using ecslite.extensions;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

namespace Game.CoreLogic
{
    public interface ILinkComponent
    {
        public int GetLink();
        public void SetLink(int link);
    }

    public struct RequestChangeLink<TLinkComponent>  where TLinkComponent : ILinkComponent
    {
        public int LinkToEntity;
    }

    public struct LinkContainer<TLinkComponent> : IListComponent<int>, IEcsAutoReset<LinkContainer<TLinkComponent>> where TLinkComponent : ILinkComponent
    {
        public List<int> Links;
        
        public void AutoReset(ref LinkContainer<TLinkComponent> c)
        {
            if (c.Links.IsNull())
            {
                c.Links = new List<int>();
            }
            else
            {
                c.Links.Clear();
            }
        }

        public List<int> GetList()
        {
            return Links;
        }
    }

    //ToDo : Make with jobs
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