using System;
using System.Collections.Generic;
using System.Linq;
using Game.CoreLogic;
using Unity;
using UnityEngine;
using ViewModel;

namespace Game.PresenterLogic
{
    public class UnifiedViewKeyPresenter : AbstractEcsPresenter<UnifiedViewKeyPresenter, IViewModel, UnifiedViewKeyComponent>
    {
        public bool IsRethrowExceptionOrCallDefault;

        public string ViewModelPlaceKey;
        [ViewKeyProperty(typeof(IViewModel))] public string DefaultViewModelKey;
        public List<Composition> Compositions = new List<Composition>();

        [Serializable]
        public struct Composition
        {
            public string UnifiedViewKey;
            [ViewKeyProperty(typeof(IViewModel))] public string ViewModelKey;
            [PresenterKeyProperty(typeof(EcsPresenterData), typeof(IViewModel))] public string PresenterKey;
        }

        private string _currentKey;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel viewModel)
        {
            base.Initialize(ecsPresenterData, viewModel);
        }

        protected override void Update(UnifiedViewKeyComponent data)
        {
            base.Update(data);
            try
            {
                if (!string.Equals(_currentKey, data.Value))
                {
                    _currentKey = data.Value;
                    var composiotion =
                        Compositions.Single(composition => string.Equals(composition.UnifiedViewKey, data.Value));
                    var presenter = PresenterResolver.Resolve<EcsPresenterData, IViewModel>(composiotion.PresenterKey);
                    var viewModel = ViewResolver.Resolve<IViewModel>(composiotion.ViewModelKey);
                    var presenterData = EcsPresenterData;
                    presenter.Initialize(presenterData, viewModel);
                    View.SetViewModel(viewModel, ViewModelPlaceKey);
                }
            }
            catch (Exception e)
            {
                if (IsRethrowExceptionOrCallDefault)
                {
                    throw e;
                }
            }
        }
    }
}