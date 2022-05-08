using System;
using Game.CoreLogic;
using UnityEngine;
using ViewModel;

namespace Game.PresenterLogic
{
    [Serializable]
    public class SelectAnalyticPresenter : AbstractEcsPresenter<SelectAnalyticPresenter, IViewModel>
    {
        public string PlaceName;
        public string StringPropertyName;

        private IViewModelEvent<string> _viewModelEvent;

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel viewModel)
        {
            base.Initialize(ecsPresenterData, viewModel);
            _viewModelEvent = viewModel.GetViewModelData<IViewModelEvent<string>>(StringPropertyName);
            var disposable = _viewModelEvent.Subscribe(AnalyticMethod);
            this.AddTo(disposable);
            viewModel.AddTo(disposable);
        }

        protected override SelectAnalyticPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.PlaceName = PlaceName;
            clone.StringPropertyName = StringPropertyName;
            return clone;
        }

        private void AnalyticMethod(string obj)
        {
            Debug.Log($"<color=green>[Analytics]</color> Place - {PlaceName}; ViewModel - {View.ToString()}; Parameter - {obj}");
        }
    }
}