using System;

namespace Game.CoreLogic
{
    [Serializable]
    public struct AppLayerLink : ILinkComponent
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