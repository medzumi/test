using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Logic.Config;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs.Utility
{
    [Serializable]
    public struct ListComponent<T> : IImportable, IEcsAutoReset<ListComponent<T>>
    {
        private static readonly string _componentName = $"ListComponent<{typeof(T).Name}>";
        
        public List<T> ComponentData;
        public void AutoReset(ref ListComponent<T> c)
        {
            if (c.ComponentData == null)
            {
                c.ComponentData = new List<T>();
            }
            else
            {
                c.ComponentData.Clear();
            }
        }

        public string ComponentName { get; }
    }

    [Serializable]
    public struct ListComponent<T, TFlag> : IImportable, IEcsAutoReset<ListComponent<T, TFlag>>
    {
        private static readonly string _componentName = $"ListComponent<{typeof(T).Name},{typeof(TFlag).Name}>";
        
        public List<T> ComponentData;

        public void AutoReset(ref ListComponent<T, TFlag> c)
        {
            if (c.ComponentData.IsNull())
            {
                c.ComponentData = new List<T>();
            }
            else
            {
                c.ComponentData.Clear();
            }
        }

        public string ComponentName => _componentName;
    }
}