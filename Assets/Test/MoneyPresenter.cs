using System;
using System.Collections.Generic;
using System.Threading;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using Game.CoreLogic;
using Leopotam.EcsLite;
using Presenting;
using UniRx;
using Utilities.Pooling;
using ViewModel;

namespace EcsViewModelPresenting
{
    [Serializable]
    public class MoneyPresenter : AbstractEcsPresenter<MoneyPresenter, MoneyComponent>
    {
        [Bind("HardValue/Money")] 
        public IViewModelProperty<int> MoneyReactiveProperty;

        public override void Initialize(EcsWorld ecsWorld, int currentEntity, EcsWorld viewModelWorld, int viewModelEntity)
        {
            base.Initialize(ecsWorld, Entity, viewModelWorld, viewModelEntity);
        }

        public override void Update(MoneyComponent data)
        {
            MoneyReactiveProperty.SetValue(data.Value);
        }

        public override string ToString()
        {
            return nameof(MoneyPresenter);
        }
    }

    public interface IEcsPresenter<TData> : IDisposable, IClonable<IEcsPresenter<TData>>
    {
        public void Initialize(EcsWorld world, int entity, EcsWorld viewModelWorld, int viewModelEntity);

        public void Update(TData data);
    }
    
    public abstract class Presenter<T> : IBindData, IDisposable where T : Presenter<T>, new()
    {
        private static readonly Pool<T> _pool;

        private bool _isDisposed = false;
        
        static Presenter()
        {
            _pool = new Pool<T>(0, () => new T());
        }

        protected Presenter()
        {
            
        }

        public static T Create()
        {
            var obj = _pool.Get();
            obj._isDisposed = false;
            return obj;
        }

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

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
}