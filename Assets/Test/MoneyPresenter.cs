using System;
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
        void Initialize(EcsWorld ecsWorld, int currentEntity, CancellationToken cancellationToken);
        void Update(TPresentData presentData);
    }
}