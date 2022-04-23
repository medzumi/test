using System;
using System.Text;
using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Config;
using EcsViewModelPresenting;
using Leopotam.EcsLite;

namespace Game.CoreLogic
{
    [Serializable]
    [Component]
    public struct MoneyComponent : IImportable
    {
        public int Value;
        public string ComponentName => "MoneyComponent";
    }

    public interface IEcsPresenter<TData> : IDisposable, IEcsPresenter
    {
        public void Update(TData data);
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
}