using System;
using System.Collections.Generic;
using System.Linq;
using Game.CoreLogic;
using Unity;
using UnityEngine;

namespace Game.PresenterLogic
{
    public class UnifiedViewKeyPresenter : AbstractEcsPresenter<UnifiedViewKeyPresenter, UnifiedViewKeyComponent>
    {
        public bool IsRethrowExceptionOrCallDefault;

        public string ViewModelPlaceKey;
        [MonoViewModelKeyProperty] public string DefaultViewModelKey;
        public List<Composition> Compositions = new List<Composition>();

        [Serializable]
        public struct Composition
        {
            public string UnifiedViewKey;
            [MonoViewModelKeyProperty] public string ViewModelKey;
            [PresenterKeyProperty] public string PresenterKey;
        }

        private string _currentKey;

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
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
                    var presenter = PresenterResolver.Resolve(composiotion.PresenterKey);
                    var viewModel = ViewModelResolver.Resolve(composiotion.ViewModelKey);
                    var presenterData = EcsPresenterData;
                    presenterData.ViewModel = viewModel;
                    presenter.Initialize(presenterData);
                    EcsPresenterData.ViewModel.SetViewModel(viewModel, ViewModelPlaceKey);
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