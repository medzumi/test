using Game.CoreLogic;
using Leopotam.EcsLite;
using Utilities.Pooling;
using ViewModel;
using Zenject;

namespace Game.PresenterLogic
{
    public class ExampleScenePresenterInstaller : MonoInstaller<ExampleScenePresenterInstaller>
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            Container.Bind<IPresenter>()
                .WithId("NamePresenter")
                .To<NamePresenter>()
                .OnInstantiated()
                .AsTransient();
        }
    }

    public interface IUpdatable<TData>
    {
        void Update(TData data);
    }

    public class LinkPresenter<LinkComponent> : IPresenter, IUpdatable<LinkContainer<LinkComponent>>
        where LinkComponent : ILinkComponent
    {
        private IPool<IViewModel> _viewModelPools;
        private IFactory<IPresenter> _presenterFactory;
        
        public void Disconnect()
        {
            
        }

        public void Update(LinkContainer<LinkComponent> data)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class CategoryPresenter : IPresenter, IUpdatable<CategoryComponent>
    {
        private readonly IViewModelProperty<string> _categoryProperty;

        public CategoryPresenter(IViewModel viewModel)
        {
            _categoryProperty = viewModel.GetViewModelData<IViewModelProperty<string>>("Category");
        }

        public void Disconnect()
        {
        }

        public void Update(CategoryComponent data)
        {
            _categoryProperty.SetValue(data.Value);
        }
    }

    public class NamePresenter : IPresenter, IUpdatable<NameComponent>
    {
        private readonly IViewModelProperty<string> _nameProperty;

        public NamePresenter(IViewModel viewModel)
        {
            _nameProperty = viewModel.GetViewModelData<IViewModelProperty<string>>("Name");
        }

        public void Update(NameComponent data)
        {
            _nameProperty.SetValue(data.Value);
        }

        public void Disconnect()
        {
            
        }
    }

    public readonly struct EcsPresenterData
    {
        public readonly EcsWorld World;
        public readonly int Entity;

        public EcsPresenterData(EcsWorld world, int entity)
        {
            World = world;
            Entity = entity;
        }
    }
}