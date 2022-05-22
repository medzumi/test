using System;

namespace Game.CoreLogic
{
    [Serializable]
    public struct DefaultLink : ILinkComponent
    {
        public int Link;
        public int GetLink()
        {
            return Link;
        }

        public void SetLink(int link)
        {
            Link = link;
        }
    }
}