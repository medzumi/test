using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using EcsViewModelPresenting;
using Leopotam.EcsLite;
using Utilities.GenericPatterns;

namespace Game.CoreLogic
{
    [Serializable]
    [Component]
    public struct MoneyComponent : IImportapleComponent
    {
        public int Value;
        public string ComponentName => "MoneyComponent";
    }

    public interface ILinkableComponent
    {
        IEnumerable<int> GetLinks();
    }

    public class PresenterResolver
    {
        public IEcsPresenter<TData> Resolve<TData>(string key)
        {
            return null;    
        }
    }
    
    [Serializable]
    public class LinkPresenter<TData> : AbstractEcsPresenter<LinkPresenter<TData>, TData>
        where TData : ILinkableComponent
    {
        private readonly List<string> _binders = new List<string>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private int _viewModelEntity = -1;

        public override void Initialize(EcsWorld world, int entity, EcsWorld viewModelWorld, int viewModelEntity)
        {
            base.Initialize(world, entity,viewModelWorld, viewModelEntity);
            _viewModelEntity = viewModelEntity;
        }

        public override void Update(TData data)
        {
            base.Update(data);
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            CompareLists();
            int index = 0;
            foreach (var entity in data.GetLinks())
            {
                var presenter = Singletone<PresenterResolver>.instance.Resolve<TData>(_binders[index]);
                presenter.Initialize(EcsWorld, entity, EcsWorld, _viewModelEntity);
                _disposables[index] = presenter;
            }
        }

        private void CompareLists()
        {
            for (int i = _binders.Count; i < _disposables.Count; i++)
            {
                _disposables.Add(null);
            }
        }

        protected override LinkPresenter<TData> CloneHandler()
        {
            var clone = base.CloneHandler();
            clone._binders.AddRange(_binders);
            return clone;
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            _binders.Clear();
        }

        public string ComponentName => $"LinkPresenter<{typeof(TData).Name}>";
    }
}