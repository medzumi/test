using System;
using Game.CoreLogic;
using Unity;

namespace Game.PresenterLogic
{
    [Serializable]
    public class RootPresenter : AbstractEcsPresenter<RootPresenter>
    {
        [PresenterKeyProperty] public string PresenterKey;
        [MonoViewModelKeyProperty] public string MonoViewModelKey;
        
        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
        }
    }
}