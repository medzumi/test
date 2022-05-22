namespace Game.PresenterLogic
{
    public interface IWindowsPresenter
    {
        void OpenWindow<TModel>(string key, TModel model);
    }
}