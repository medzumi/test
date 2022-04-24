using System;
using System.Collections.Generic;

namespace Game.CoreLogic
{
    [Serializable]
    public struct InteractComponent : ILinkableComponent
    {
        public int Player;
        public int Trader;
        
        public IEnumerable<int> GetLinks()
        {
            yield return Player;
            yield return Trader;
        }
    }
}