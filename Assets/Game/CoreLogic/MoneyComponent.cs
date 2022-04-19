using System;
using ApplicationScripts.Logic.Config;
using Leopotam.EcsLite;

namespace Game.CoreLogic
{
    [Serializable]
    [Component]
    public struct MoneyComponent : IImportapleComponent
    {
        public int Value;
        public string ComponentName => "MoneyComponent";
    }
}