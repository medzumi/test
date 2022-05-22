using Leopotam.EcsLite;

namespace Game.CoreLogic.Rewarding
{
    public interface ITargetActionSystem : IEcsSystem
    {
        void DoReward(int rewardEntity);
    }
}