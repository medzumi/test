using Game.CoreLogic;
using presenting.ecslite;
using ViewModel;

namespace Game.PresenterLogic
{
    public class NamePresenter : AbstractPresenter<NamePresenter, IViewModel, NameComponent>
    {
        public string PropertyKey;

        private IViewModelProperty<string> _nameProperty;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel view)
        {
            base.Initialize(ecsPresenterData, view);
            _nameProperty = view.GetViewModelData<IViewModelProperty<string>>(PropertyKey);
        }

        protected override void Update(NameComponent data)
        {
            base.Update(data);
            _nameProperty.SetValue(data.Value);
        }

        protected override NamePresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.PropertyKey = PropertyKey;
            return clone;
        }
    }
}