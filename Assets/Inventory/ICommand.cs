namespace Inventory
{
    public interface ICommand
    {
        void Apply();
        void Revert();
    }
}