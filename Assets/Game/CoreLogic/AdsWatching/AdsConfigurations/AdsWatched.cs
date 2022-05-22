using Game.CoreLogic.Rewarding;

namespace Game.CoreLogic.AdsConfigurations
{
    public struct AdsWatched : ICount
    {
        public int count;
        public int Count => count;
    }
}