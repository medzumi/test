using System.Collections.Generic;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

public struct InventoryData : IEcsAutoReset<InventoryData>
{
    public List<int> Data;

    public void AutoReset(ref InventoryData c)
    {
        if (c.Data.IsNull())
        {
            c.Data = new List<int>();
        }
        else
        {
            c.Data.Clear();
        }
    }
}