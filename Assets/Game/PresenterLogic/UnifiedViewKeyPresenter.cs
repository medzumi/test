using System;
using System.Collections.Generic;
using System.Linq;
using Game.CoreLogic;
using presenting.ecslite;
using Unity;
using UnityEngine;
using unityPresenting.Unity;
using ViewModel;

namespace Game.PresenterLogic
{
    [Serializable]
    public struct Composition
    {
        public string UnifiedViewKey;
        [ViewKeyProperty(typeof(IViewModel))] public string ViewModelKey;
        [PresenterKeyProperty(typeof(EcsPresenterData), typeof(IViewModel))] public string PresenterKey;
    }
    
    
    public class UnifiedViewKeyPresenter : AbstractPresenter<UnifiedViewKeyPresenter, IViewModel, UnifiedViewKeyComponent>
    {
        public Composition DefaultComposition;
        public List<Composition> Compositions = new List<Composition>();

        public string ViewPropertyKey;

        private Composition _composition;
        private ViewViewModelData<IViewModel> _viewProperty;

        protected override UnifiedViewKeyPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.ViewPropertyKey = ViewPropertyKey;
            clone.Compositions = Compositions;
            clone.DefaultComposition = DefaultComposition;
            return clone;
        }

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel viewModel)
        {
            base.Initialize(ecsPresenterData, viewModel);
            _viewProperty = viewModel.GetViewModelData<ViewViewModelData<IViewModel>>(ViewPropertyKey);
        }

        public override void Update(UnifiedViewKeyComponent? data)
        {
            base.Update(data);
            var composition = DefaultComposition;
            if (data.HasValue)
            {
                composition = Compositions.Single(comp => string.Equals(comp.UnifiedViewKey, data.Value.Value));
            }

            if (!string.Equals(_composition.UnifiedViewKey, composition.UnifiedViewKey))
            {
                _composition = composition;
                _viewProperty.Fill(RequestView, true);
            }
        }

        private IViewModel RequestView()
        {
            var presenter = PresenterResolver.Resolve<EcsPresenterData, IViewModel>(_composition.PresenterKey);
            var viewModel = ViewResolver.Resolve<IViewModel>(_composition.ViewModelKey);
            var presenterData = EcsPresenterData;
            presenter.Initialize(presenterData, viewModel);

            return viewModel;
        }
    }
}