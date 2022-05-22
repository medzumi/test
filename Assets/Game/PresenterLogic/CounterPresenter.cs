using Game.CoreLogic;
using Game.CoreLogic.Rewarding;
using presenting.ecslite;
using ViewModel;

namespace Game.PresenterLogic
{
    public class CounterPresenter : AbstractPresenter<CounterPresenter, IViewModel>
    {
        public string CountKey;

        private IViewModelProperty<int> _viewModelProperty;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel view)
        {
            base.Initialize(ecsPresenterData, view);
            _viewModelProperty = view.GetViewModelData<IViewModelProperty<int>>(CountKey);
        }

     /*   protected override void Update(AvailableRewardsCount data)
        {
            base.Update(data);
            _viewModelProperty.SetValue(data.Count);
        }*/

        protected override CounterPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.CountKey = CountKey;
            return clone;
        }
    }
}