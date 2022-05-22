using System;
using System.Collections.Generic;
using System.Linq;
using presenting.ecslite;
using presenting.Unity.Default;
using ViewModel;
using ViewModel.Unity;

namespace Game.PresenterLogic
{
    [Serializable]
    public class WindowsPresenter : AbstractPresenter<WindowsPresenter, IViewModel>, IWindowsPresenter, IInject<IViewModelWindowService>
    {
        public string SubWindowsViewPropertyKey;

        public List<Composition> WindowsCompositions = new List<Composition>();
        
        private IViewModelWindowService _windowService;

        private ViewViewModelData<IViewModel> _subWindowsViewPropertyKey;
        private IViewModel _currentWindow;
        private Composition? _currentComposition;
        private EcsPresenterData _currentData;

        protected override WindowsPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.SubWindowsViewPropertyKey = SubWindowsViewPropertyKey;
            clone._windowService = _windowService;
            clone.WindowsCompositions = WindowsCompositions;
            return clone;
        }

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel view)
        {
            base.Initialize(ecsPresenterData, view);
            _subWindowsViewPropertyKey = view.GetViewModelData<ViewViewModelData<IViewModel>>(SubWindowsViewPropertyKey);
            _subWindowsViewPropertyKey.Fill(FillRequest, true);
            _windowService.RegisterWindow(view, this);
        }

        protected override void DisposeHandler()
        {
            _windowService.UnregisterWindow(View);
            base.DisposeHandler();
        }

        private IViewModel FillRequest()
        {
            if (_currentComposition.HasValue)
            {
                var view = ViewResolver.Resolve<IViewModel>(_currentComposition.Value.ViewModelKey);
                var presenter =
                    PresenterResolver.Resolve<EcsPresenterData, IViewModel>(_currentComposition.Value.PresenterKey);
                presenter.Initialize(_currentData, view);

                return view;
            }

            return null;
        }

        public void Inject(IViewModelWindowService injectable)
        {
            _windowService = injectable;
        }

        public void OpenWindow<TModel>(string key, TModel model)
        {
            _currentWindow?.Dispose();
            var newWindowComposition =
                WindowsCompositions.Single(composition => string.Equals(composition.UnifiedViewKey, key));
            if (model is EcsPresenterData presenterData)
            {
                _currentComposition = newWindowComposition;
                _currentData = presenterData;
                _subWindowsViewPropertyKey.Fill(FillRequest, true);
            }
            else
            {
                throw new Exception("This windows presenter support only EcsPresenterData as Model");
            }
        }
    }
}