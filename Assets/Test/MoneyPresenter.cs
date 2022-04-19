using System;
using System.Collections.Generic;
using System.Threading;
using Game.CoreLogic;
using Leopotam.EcsLite;
using Presenting;
using ViewModel;

namespace EcsViewModelPresenting
{
    [Serializable]
    public struct MoneyPresenter : IBindData, IEcsPresenter<MoneyComponent>
    {
        [Bind("HardValue/Money")] 
        public IViewModelProperty<int> MoneyReactiveProperty;

        public void Initialize(EcsWorld ecsWorld, int currentEntity, CancellationToken cancellationToken)
        {

        }

        public void Update(MoneyComponent data)
        {
            MoneyReactiveProperty.SetValue(data.Value);
        }

        public override string ToString()
        {
            return nameof(MoneyPresenter);
        }
    }

    public interface IEcsPresenter<TPresentData>
    {
        void Initialize(EcsWorld ecsWorld, int currentEntity);
        void Update(TPresentData presentData);
    }

    public struct PresenterHandler<TPresenter, TPresentData> : IEcsPresenter<TPresentData>, IEcsAutoReset<PresenterHandler<TPresenter,TPresentData>>
        where TPresenter : IEcsPresenter<TPresentData>
    {
        public List<IDisposable> Disposables;

        public Dictionary<IViewModel, TPresenter> Presenters;

        public void AutoReset(ref PresenterHandler<TPresenter, TPresentData> presenterHandler)
        {
            if (presenterHandler.Disposables.IsNull())
            {
                presenterHandler.Disposables = new List<IDisposable>();
            }
            else
            {
                foreach (var disposable in presenterHandler.Disposables)
                {
                    disposable.Dispose();
                }

                presenterHandler.Disposables.Clear();
            }

            if (presenterHandler.Presenters.IsNull())
            {
                presenterHandler.Presenters = new Dictionary<IViewModel, TPresenter>();
            }
            else
            {
                presenterHandler.Presenters.Clear();
            }
        }

        public void Initialize(EcsWorld ecsWorld, int currentEntity)
        {
            
        }

        public void Update(TPresentData presentData)
        {
            
        }
    }
}