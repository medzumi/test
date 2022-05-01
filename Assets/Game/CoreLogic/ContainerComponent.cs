using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using ecslite.extensions;
using Leopotam.EcsLite;

namespace Game.CoreLogic
{
    [Serializable]
    public struct ContainerComponent : IListComponent<int>, IEcsAutoReset<ContainerComponent>
    {
        public List<int> List;

        public List<int> GetList()
        {
            return List;
        }

        public void AutoReset(ref ContainerComponent c)
        {
            if (List.IsNull())
            {
                List = new List<int>();
            }
            else
            {
                List.Clear();
            }
        }
    }
}