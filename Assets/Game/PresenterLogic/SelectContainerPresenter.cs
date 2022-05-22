using System;
using Game.CoreLogic;
using Game.PresenterLogic;
using presenting.ecslite;
using Unity;
using ViewModel;

namespace Game.View
{
    [Serializable]
    public class SelectContainerPresenter : AbstractPresenter<SelectContainerPresenter, IViewModel, LinkContainer<DefaultLink>>
    {
        public ContainerPresenter FirstContainerPresenter = ContainerPresenter.Create();
        public ContainerPresenter SecondContainerPresenter = ContainerPresenter.Create();

        public string FirstContainerSelectEvent;
        public string SecondContainerSelectEvent;

        private bool _isRaisedEvent = false;
        
        private IViewModelEvent<string> _firstContainerEvent;
        private IViewModelEvent<string> _secondContainerEvent;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel viewModel)
        {
            base.Initialize(ecsPresenterData, viewModel);
            FirstContainerPresenter.Initialize(ecsPresenterData, viewModel);
            SecondContainerPresenter.Initialize(ecsPresenterData, viewModel);
            _firstContainerEvent = viewModel.GetViewModelData<IViewModelEvent<string>>(FirstContainerSelectEvent);
            _secondContainerEvent = viewModel.GetViewModelData<IViewModelEvent<string>>(SecondContainerSelectEvent);

            var disposable = _firstContainerEvent.Subscribe(FirstEventAction);
            this.AddTo(disposable);
            viewModel.Subscribe(disposable);

            disposable = _secondContainerEvent.Subscribe(SecondEventAction);
            this.AddTo(disposable);
            viewModel.Subscribe(disposable);
        }

        protected override SelectContainerPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.FirstContainerPresenter = (ContainerPresenter)FirstContainerPresenter.Clone();
            clone.SecondContainerPresenter = (ContainerPresenter)SecondContainerPresenter.Clone();
            clone.FirstContainerPresenter.PresenterResolver = PresenterResolver;
            clone.FirstContainerPresenter.ViewResolver = ViewResolver;
            clone.SecondContainerPresenter.PresenterResolver = PresenterResolver;
            clone.SecondContainerPresenter.ViewResolver = ViewResolver;
            clone.FirstContainerSelectEvent = FirstContainerSelectEvent;
            clone.SecondContainerSelectEvent = SecondContainerSelectEvent;

            return clone;
        }

        private void SecondEventAction(string obj)
        {
            if (_isRaisedEvent)
            {
                return;
            }

            _isRaisedEvent = true;
            _firstContainerEvent.SetValue(obj);
            _isRaisedEvent = false;
        }

        private void FirstEventAction(string obj)
        {
            if (_isRaisedEvent)
            {
                return;
            }

            _isRaisedEvent = true;
            _secondContainerEvent.SetValue(obj);
            _isRaisedEvent = false;
        }
    }
}