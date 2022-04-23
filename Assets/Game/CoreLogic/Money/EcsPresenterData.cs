using System;
using Leopotam.EcsLite;
using ViewModel;

namespace Game.CoreLogic
{
    [Serializable]
    public struct EcsPresenterData
    {
        public EcsWorld ModelWorld;
        public int ModelEntity;
        public IViewModel ViewModel;
    }
}