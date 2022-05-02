using System;
using Game.CoreLogic;
using Presenter;
using ViewModel;

namespace Game.PresenterLogic
{
    [Serializable]
    public class ContainerPresenter : EntityListPresenter<ContainerPresenter, ContainerComponent>
    {
        
        
        protected override IViewModel ResolveElementViewModel(int arg)
        {
            return base.ResolveElementViewModel(arg);
        }
    }
}