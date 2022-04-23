namespace Game.CoreLogic
{
    public interface IPresenterResolver
    {
        public IEcsPresenter Resolve(string key);
    }
}