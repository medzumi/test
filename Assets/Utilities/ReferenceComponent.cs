using ApplicationScripts.Logic.Config;

namespace ApplicationScripts.Ecs.Utility
{
    public struct ReferenceComponent<TReference> : IImportapleComponent where TReference : class
    {
        private static readonly string _componentName = $"ReferenceComponent<{typeof(TReference).Name}>"; 
        
        public TReference reference;
        public string ComponentName { get; }
    }
}