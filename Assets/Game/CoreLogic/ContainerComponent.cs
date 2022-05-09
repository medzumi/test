using System;
using System.Collections.Generic;
using ecslite.extensions;
using Leopotam.EcsLite;
using Utilities.CodeExtensions;

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