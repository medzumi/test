using presenting.ecslite;
using presenting.Unity.Default;
using ViewModel;

namespace Game.PresenterLogic
{
    public class OpenSubWindowPresenter : AbstractPresenter<OpenSubWindowPresenter, IViewModel>, IInject<IViewModelWindowService>
    {
        public string EventKey;
        public string WindowKey;

        private IViewModelWindowService _windowService;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel view)
        {
            base.Initialize(ecsPresenterData, view);
            var disposable = view.GetViewModelData<IViewModelEvent<NullData>>(EventKey)
                .Subscribe(EventMethod);
            AddTo(disposable);
        }

        private void EventMethod(NullData obj)
        {
            _windowService?.ShowSubWindow(WindowKey, EcsPresenterData, View);
        }

        public void Inject(IViewModelWindowService injectable)
        {
            _windowService = injectable;
        }

        protected override OpenSubWindowPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.EventKey = EventKey;
            clone.WindowKey = WindowKey;
            clone._windowService = _windowService;
            return clone;
        }
    }
}