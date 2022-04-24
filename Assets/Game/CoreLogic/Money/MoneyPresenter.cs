using System;
using ViewModel;

namespace Game.CoreLogic
{
    [Serializable]
    public class MoneyPresenter : AbstractEcsPresenter<MoneyPresenter, MoneyComponent>
    {
        public string MoneyKey;
        
        public IViewModelProperty<int> MoneyReactiveProperty;

        public override void Initialize(PresenterData presenterData)
        {
            MoneyReactiveProperty = presenterData.ViewModel.GetViewModelData<IViewModelProperty<int>>(MoneyKey);
            base.Initialize(presenterData);
        }

        public override void Update(MoneyComponent data)
        {
            MoneyReactiveProperty.SetValue(data.Value);
        }
    }
}