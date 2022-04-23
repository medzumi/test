using System.Collections.Generic;

namespace Game.CoreLogic
{
    public interface ILinkableComponent
    {
        IEnumerable<int> GetLinks();
    }
}