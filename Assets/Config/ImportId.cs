using System.Collections.Generic;
using ApplicationScripts.Logic.Features.Indexing;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Config
{
    public struct ImportableComponent : IIndexComponent<string>
    {
        public string Index;

        public string GetIndex()
        {
            return Index;
        }

        public string ComponentName => "ImportableComponent";
    }

    public struct ImportCommand : IEcsAutoReset<ImportCommand>
    {
        public List<string> From;
        
        public void AutoReset(ref ImportCommand c)
        {
        }
    }
}