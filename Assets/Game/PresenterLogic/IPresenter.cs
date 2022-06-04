namespace Game.PresenterLogic
{
    public interface IPresenter
    {
        void Disconnect();
    }

    public interface IComponentView<TComponent>
    {
        void Update(TComponent component);
    }
}