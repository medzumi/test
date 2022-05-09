using System;
using Game.CoreLogic;
using presenting.ecslite;
using Unity;
using unityPresenting.Unity;
using ViewModel;

namespace Game.PresenterLogic
{
    [Serializable]
    public class RootPresenter : AbstractPresenter<RootPresenter, IViewModel>
    {
        [PresenterKeyProperty(typeof(EcsPresenterData), typeof(IViewModel))] public string PresenterKey;
        [ViewKeyProperty(typeof(IViewModel))] public string MonoViewModelKey;
        
        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel viewModel)
        {
            base.Initialize(ecsPresenterData, viewModel);
        }
    }
}