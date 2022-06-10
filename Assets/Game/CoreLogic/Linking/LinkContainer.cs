using System.Collections.Generic;
using ecslite.extensions;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

namespace Game.CoreLogic
{
    public struct LinkContainer<TLinkComponent> : IListComponent<int>, IEcsAutoReset<LinkContainer<TLinkComponent>> where TLinkComponent : ILinkComponent
    {
        public List<int> Links;
        
        public void AutoReset(ref LinkContainer<TLinkComponent> c)
        {
            if (c.Links.IsNull())
            {
                c.Links = new List<int>();
            }
            else
            {
                c.Links.Clear();
            }
        }

        public List<int> GetList()
        {
            return Links;
        }
    }
}