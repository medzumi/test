using System;
using Game.CoreLogic;
using presenting.ecslite.ViewModelPresenters;
using ViewModel;

namespace Game.PresenterLogic
{
    [Serializable]
    public class ContainerPresenter : EntityListPresenter<ContainerPresenter, LinkContainer<DefaultLink>>
    {
        protected override IViewModel ResolveElementViewModel(int arg)
        {
            return base.ResolveElementViewModel(arg);
        }
    }
}