using ViewModel;

namespace Game.PresenterLogic
{
    public interface IViewModelWindowService
    {
        void ShowSubWindow<TModel>(string key, TModel model, IViewModel viewModel);

        void UnregisterWindow(IViewModel viewModel);
        
        void RegisterWindow(IViewModel viewModel, IWindowsPresenter windowsPresenter);
    }
}