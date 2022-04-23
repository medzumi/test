using System;
using ApplicationScripts.Ecs;
using Leopotam.EcsLite;
using Utilities.GenericPatterns;

namespace Game.CoreLogic
{
    [Serializable]
    public abstract class AbstractEcsPresenter<TPresenter> : PoolableObject<TPresenter>, IEcsPresenter
        where TPresenter : AbstractEcsPresenter<TPresenter>, new()
    {
        private EcsPresenterData _ecsPresenterData;

        protected EcsPresenterData EcsPresenterData
        {
            get => _ecsPresenterData;
            private set => _ecsPresenterData = value;
        }
        protected EcsPool<DisposeComponent> DisposeComponentPool { get; private set; }

        protected IEcsPresenter Resolve(string key)
        {
            return Singletone<IPresenterResolver>.instance.Resolve(key);
        }
        
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
}