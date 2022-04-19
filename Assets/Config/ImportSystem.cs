using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Features.Indexing;
using EcsViewModelPresenting;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Presenting;
using ViewModel;

namespace ApplicationScripts.Logic.Config
{
    public class ImportSystem : SystemCollection
    {
        private EcsPool<ImportCommand> _importCommandPool;
        private EcsPool<ReferenceComponent<JObject>> _importableEntityPool;
        private EcsFilter _filter;
        private IndexedEntityLibrarySystem<ImportableComponent, string> _librarySystem;
        private readonly List<string> _subImportBuffer = new List<string>();
        private readonly List<JObject> _importableEntities;
        private EcsPool<InternalImportCommand> _importListPool;
        private EcsFilter _filter2;
        private EcsFilter _filter3;

        public ImportSystem(List<JObject> importableEntities)
        {
            _importableEntities = importableEntities;
            Add(new ImportImportableEntity());
        }

        public override void PreInit(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _importCommandPool = world.GetPool<ImportCommand>();
            _importableEntityPool = world.GetPool<ReferenceComponent<JObject>>();
            _importListPool = world.GetPool<InternalImportCommand>();
            _filter = world.Filter<ImportCommand>().End();
            _librarySystem = IndexedEntityLibrarySystem<ImportableComponent, string>.GetLibrary(world);
            _filter2 = world.Filter<InternalImportCommand>().End();
            _filter3 = world.Filter<ReferenceComponent<JObject>>().End();
            foreach (var entity in _importableEntities)
            {
                var newEntity = world.NewEntity();
                ref var a = ref _importableEntityPool.Add(newEntity);
                a.reference = entity;
            }
            base.PreInit(systems);
            foreach (var entity in _filter3)
            {
                _importableEntityPool.Del(entity);
            }
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                _subImportBuffer.Clear();
                _subImportBuffer.AddRange(_importCommandPool.Read(entity).From);
                for(int i = 0; i < _subImportBuffer.Count; i++)
                {
                    var importableEntityIndex = _subImportBuffer[i];
                    var importableEntity = _librarySystem.GetEntity(importableEntityIndex);
                    _importListPool.Ensure(importableEntity)
                        .Ids
                        .Add(entity);
                }
            }
            base.Run(systems);
            _importCommandPool.Clear();
            _importListPool.Clear();
        }
    }

    internal struct InternalImportCommand : IEcsAutoReset<InternalImportCommand>
    {
        public List<int> Ids;

        public void AutoReset(ref InternalImportCommand c)
        {
            if (c.Ids.IsNull())
            {
                c.Ids = new List<int>();
            }
            else
            {
                c.Ids.Clear();
            }
        }
    }

    public interface IImportapleComponent
    {
        public string ComponentName { get; }
    }

    public class ImportImportableEntity : IEcsRunSystem, IEcsPreInitSystem
    {
        private static ImportableComponent _exampleImportType = new ImportableComponent();
        private EcsPool<ReferenceComponent<JObject>> _jObjectPool;
        private EcsPool<ImportableComponent> _importTypePool;
        private EcsPool<InternalImportCommand> _internalImportCommand;
        private EcsFilter _filter;

        public void Run(EcsSystems systems)
        {
            Import();
        }

        private void Import()
        {
            foreach (var entity in _filter)
            {
                var reference = _jObjectPool.Read(entity).reference;
                if (reference.ContainsKey(_exampleImportType.ComponentName))
                {
                    _importTypePool.Add(entity,
                        reference.GetValue(_exampleImportType.ComponentName)!.ToObject<ImportableComponent>());
                }
            }
        }

        public void PreInit(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _jObjectPool = world.GetPool<ReferenceComponent<JObject>>();
            _importTypePool = world.GetPool<ImportableComponent>();
            _filter = world.Filter<ReferenceComponent<JObject>>().End();
            _internalImportCommand = world.GetPool<InternalImportCommand>();
            Import();
        }
    }

    public struct BindComponent
    {
        public string Key;
        public int ToEntity;
    }
    
    public class BindSystem<TLogicComponent, TBindData> : EcsSystemBase
        where TLogicComponent : struct
        where TBindData : struct, IEcsPresenter<TLogicComponent>, IBindData
    {
        private IndexedEntityLibrarySystem<ImportableComponent, string> _librarySystem;
        private EcsPool<TLogicComponent> _logicComponentPool;
        private EcsPool<TBindData> _bindDataPool;
        private EcsPool<ListComponent<TBindData>> _bindDatasPool;
        private EcsPool<BindComponent> _bindPool;
        private EcsPool<ReferenceComponent<IViewModel>> _viewModelPool;
        private collector _collector;
        private collector _collector2;
        private EcsWorld _world;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            _world = systems.GetWorld();
            _logicComponentPool = _world.GetPool<TLogicComponent>();
            _bindDataPool = _world.GetPool<TBindData>();
            _viewModelPool = _world.GetPool<ReferenceComponent<IViewModel>>();
        }

        public override void Run(EcsSystems systems)
        {
            base.Run(systems);
            foreach (var entity in _collector)
            {
                var bindCommand = _bindPool.Read(entity);
                var importableEntity = _librarySystem.GetEntity(bindCommand.Key);
                if (_viewModelPool.Has(bindCommand.ToEntity))
                {
                    var viewModel = _viewModelPool.Read(bindCommand.ToEntity).reference;
                    var bindData = _bindDataPool.Read(importableEntity);
                    bindData = viewModel.GetBindData<TBindData>(bindData);
                    var disposable = viewModel.OnDispose((vm) =>
                    {
                        _bindDatasPool.Read(entity).ComponentData.Remove(bindData);
                    });
                    _bindDatasPool.Ensure(entity).ComponentData.Add(bindData);    
                }
                
                
                //GetDisposeComponent().Add   
            }
        }

        private DisposeComponent GetDisposeComponent()
        {
            return _diosposer.Count > 0 ? _diosposer.Pop() : new DisposeComponent();
        }
    }

    public class SelfBindSystem<TLogicComponent, TBindData> : EcsSystemBase
     where TLogicComponent : struct
     where TBindData : struct, IEcsPresenter<TLogicComponent>, IBindData
    {
        private EcsPool<TLogicComponent> _logicComponentPool;
        private EcsPool<TBindData> _bindDataPool;
        private EcsPool<ReferenceComponent<IViewModel>> _viewModelPool;
        private collector _collector;
        private collector _collector2;
        private EcsWorld _world;

        public override void PreInit(EcsSystems systems)
        {
            _world = systems.GetWorld();
            _logicComponentPool = _world.GetPool<TLogicComponent>();
            _bindDataPool = _world.GetPool<TBindData>();
            _collector = _world.Filter<TLogicComponent>().Inc<TBindData>()
                .EndCollector(CollectorEvent.Added | CollectorEvent.Dirt);
            _collector2 = _world.Filter<ReferenceComponent<IViewModel>>()
                .Inc<TBindData>()
                .Inc<TLogicComponent>()
                .EndCollector(CollectorEvent.Added);
            _viewModelPool = _world.GetPool<ReferenceComponent<IViewModel>>();
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _collector2)
            {
                _bindDataPool.Set(entity) = _viewModelPool
                    .Read(entity)
                    .reference
                    .GetBindData(_bindDataPool.Read(entity));
                _bindDataPool.Read(entity)
                    .Initialize(_world, entity, _viewModelPool.Read(entity).reference.CancellationToken.Token);
            }
            
            foreach (var entity in _collector)
            {
                _bindDataPool.Read(entity).Update(_logicComponentPool.Read(entity));
            }
        }
    }

    public class ImportJConvertCommandSystem<TImportType> : IEcsRunSystem, IEcsPreInitSystem
        where TImportType : struct, IImportapleComponent
    {
        private static TImportType _exampleImportType = new TImportType();
        private EcsPool<ReferenceComponent<JObject>> _jObjectPool;
        private EcsPool<TImportType> _importTypePool;
        private EcsPool<InternalImportCommand> _internalImportCommand;
        private EcsFilter _filter;
        private EcsFilter _filter2;

        public virtual void Run(EcsSystems systems)
        {
            Import();
            foreach (var entity in _filter2)
            {
                foreach (var toEntity in _internalImportCommand.Read(entity).Ids)
                {
                    _importTypePool.Ensure(toEntity) = _importTypePool.Read(entity);
                }
            }
        }

        private void Import()
        {
            foreach (var entity in _filter)
            {
                var reference = _jObjectPool.Read(entity).reference;
                if (reference.ContainsKey(_exampleImportType.ComponentName))
                {
                    _importTypePool.Add(entity,
                        reference.GetValue(_exampleImportType.ComponentName)!.ToObject<TImportType>());
                }
            }
        }

        public void PreInit(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _jObjectPool = world.GetPool<ReferenceComponent<JObject>>();
            _importTypePool = world.GetPool<TImportType>();
            _filter = world.Filter<ReferenceComponent<JObject>>().End();
            _filter2 = world.Filter<InternalImportCommand>().Inc<TImportType>().End();
            _internalImportCommand = world.GetPool<InternalImportCommand>();
            Import();
        }
    }
}