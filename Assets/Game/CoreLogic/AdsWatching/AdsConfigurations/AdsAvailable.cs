using Game.CoreLogic.Rewarding;

namespace Game.CoreLogic.AdsConfigurations
{
    public struct AdsAvailable : ICount
    {
        public int count;
        public int Count => count;
    }
}