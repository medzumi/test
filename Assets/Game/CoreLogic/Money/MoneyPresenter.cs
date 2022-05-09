using System;
using presenting.ecslite;
using ViewModel;

namespace Game.CoreLogic
{
    [Serializable]
    public class MoneyPresenter : AbstractPresenter<MoneyPresenter, IViewModel, MoneyComponent>
    {
        public string MoneyKey;
        
        public IViewModelProperty<int> MoneyReactiveProperty;

        public override void Initialize(EcsPresenterData presenterData, IViewModel viewModel)
        {
            MoneyReactiveProperty = viewModel.GetViewModelData<IViewModelProperty<int>>(MoneyKey);
            base.Initialize(presenterData, viewModel);
        }

        protected override void Update(MoneyComponent data)
        {
            MoneyReactiveProperty.SetValue(data.Value);
        }

        protected override MoneyPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.MoneyKey = MoneyKey;
            return clone;
        }
    }
}