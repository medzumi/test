using ApplicationScripts.Logic.Features.Indexing;

namespace Inventory
{
    public struct InventoryIndex : IIndexComponent<int>
    {
        public int Index;
        
        public int GetIndex()
        {
            return Index;
        }
    }
}