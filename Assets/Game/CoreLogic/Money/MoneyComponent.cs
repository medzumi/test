using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using EcsViewModelPresenting;
using Leopotam.EcsLite;
using Utilities.GenericPatterns;
using Utilities.Pooling;
using ViewModel;

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

    [Serializable]
    public struct TradeComponent : ILinkableComponent
    {
        public int Player;
        public int Trader;
        
        public IEnumerable<int> GetLinks()
        {
            yield return Player;
            yield return Trader;
        }
    }
    
    [Serializable]
    public class MoneyPresenter : AbstractEcsPresenter<MoneyPresenter, MoneyComponent>
    {
        public IViewModelProperty<int> MoneyReactiveProperty;

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            MoneyReactiveProperty = ecsPresenterData.ViewModel.GetViewModelData<IViewModelProperty<int>>("Money");
            base.Initialize(ecsPresenterData);
        }

        public override void Update(MoneyComponent data)
        {
            MoneyReactiveProperty.SetValue(data.Value);
        }
    }

    public interface IEcsPresenter : IDisposable, IClonable<IEcsPresenter>
    {
        public void Initialize(EcsPresenterData ecsPresenterData);
    }
    
    public interface IEcsPresenter<TData> : IDisposable, IEcsPresenter
    {
        public void Update(TData data);
    }

    public struct EcsPresenterData
    {
        public EcsWorld ModelWorld;
        public int ModelEntity;
        public IViewModel ViewModel;
    }
    
    public abstract class PoolableObject<T> : IDisposable where T : PoolableObject<T>, new()
    {
        private static readonly Pool<T> _pool;

        private bool _isDisposed = false;
        
        static PoolableObject()
        {
            _pool = new Pool<T>(0, () => new T());
        }

        protected PoolableObject()
        {
            
        }

        public static T Create()
        {
            var obj = _pool.Get();
            obj._isDisposed = false;
            return obj;
        }

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public IDisposable AddTo(IDisposable disposable)
        {
            _disposables.Add(disposable);
            return disposable;
        }
        
        public void Dispose()
        {
            if (!_isDisposed)
            {
                foreach (var disposable in _disposables)
                {
                    disposable?.Dispose();
                }    
                _disposables.Clear();
                DisposeHandler();
                _isDisposed = true;
                _pool.Release(this as T);
            }
        }

        protected virtual void DisposeHandler()
        {
        }
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
                presenter.Initialize(EcsPresenterData);
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

    public abstract class AbstractEcsPresenter<TPresenter> : PoolableObject<TPresenter>, IEcsPresenter
        where TPresenter : AbstractEcsPresenter<TPresenter>, new()
    {
        protected EcsPresenterData EcsPresenterData { get; private set; }
        protected EcsPool<DisposeComponent> DisposeComponentPool { get; private set; }
        
        public virtual void Initialize(EcsPresenterData ecsPresenterData)
        {
            EcsPresenterData = ecsPresenterData;
            DisposeComponentPool = ecsPresenterData.ModelWorld.GetPool<DisposeComponent>();
            DisposeComponentPool.Ensure(ecsPresenterData.ModelEntity).Disposables.Add(this);
            ecsPresenterData.ViewModel.AddTo(this);
        }
        
        
        public IEcsPresenter Clone()
        {
            return CloneHandler();
        }

        protected virtual TPresenter CloneHandler()
        {
            return AbstractEcsPresenter<TPresenter>.Create();
        }
    }


    public struct DisposeComponent : IEcsAutoReset<DisposeComponent>
    {
        public List<IDisposable> Disposables;

        public void AutoReset(ref DisposeComponent c)
        {
            if (c.Disposables.IsNull())
            {
                c.Disposables = new List<IDisposable>();
            }
            else
            {
                foreach (var disposable in c.Disposables)
                {
                    disposable?.Dispose();
                }
                c.Disposables.Clear();
            }
        }
    }

        public abstract class AbstractEcsPresenter<TPresenter, TData> : AbstractEcsPresenter<TPresenter>, IEcsPresenter<TData>
        where TPresenter : AbstractEcsPresenter<TPresenter, TData>, new()
    {
        protected EcsPool<ListComponent<IEcsPresenter<TData>>> _presentersPool;

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
            _presentersPool = ecsPresenterData.ModelWorld.GetPool<ListComponent<IEcsPresenter<TData>>>();
            _presentersPool.Ensure(ecsPresenterData.ModelEntity).ComponentData.Add(this);
        }

        public virtual void Update(TData data)
        {
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            var data = _presentersPool.Read(EcsPresenterData.ModelEntity).ComponentData;
            data.Remove(this);
            if (data.Count == 0)
            {
                _presentersPool.Del(EcsPresenterData.ModelEntity);
            }
        }
    }

    public sealed class AggregatePresenter : AbstractEcsPresenter<AggregatePresenter>
    {
        private List<IEcsPresenter> _presenters = new List<IEcsPresenter>();

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
            foreach (var ecsPresenter in _presenters)
            {
                ecsPresenter.Initialize(ecsPresenterData);
            }
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            foreach (var ecsPresenter in _presenters)
            {
                ecsPresenter.Dispose();
            }
            _presenters.Clear();
        }

        protected override AggregatePresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            foreach (var ecsPresenter in _presenters)
            {
                clone._presenters.Add(ecsPresenter.Clone());
            }

            return clone;
        }
    }
}