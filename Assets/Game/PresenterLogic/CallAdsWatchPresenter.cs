using System;
using Game.CoreLogic;
using Leopotam.EcsLite;
using presenting.ecslite;
using UnityEngine;
using ViewModel;

namespace Game.PresenterLogic
{
    public class CallAdsWatchPresenter : AbstractPresenter<CallAdsWatchPresenter, IViewModel>
    {
        public string PurchaseCommandKey;
        
        private EcsPool<AdsStartWatchComponent> _ecsPool;
        private Action<NullData> _action;

        public CallAdsWatchPresenter() : base()
        {
            _action = Action; 
        }

        private void Action(NullData obj)
        {
            _ecsPool.Add(EcsPresenterData.ModelEntity);
        }

        public override void Initialize(EcsPresenterData ecsPresenterData, IViewModel view)
        {
            base.Initialize(ecsPresenterData, view);
            _ecsPool = ecsPresenterData.ModelWorld.GetPool<AdsStartWatchComponent>();
            var disposable = view.GetViewModelData<IViewModelEvent<NullData>>(PurchaseCommandKey)
                .Subscribe(_action);
            this.AddTo(disposable);
        }

        protected override CallAdsWatchPresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            clone.PurchaseCommandKey = PurchaseCommandKey;
            return clone;
        }
    }
}