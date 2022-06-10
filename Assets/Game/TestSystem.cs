using System;
using ApplicationScripts.Ecs;
using Game.CoreLogic;
using Game.CoreLogic.Rewarding;
using Leopotam.EcsLite;
using UnityEngine;
using runtime;

namespace Game
{
    [Serializable]
    public class TestSystem : EcsSystemBase
    {
        [SerializeField] private MonoViewModel _monoViewModel;

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            var world = systems.GetWorld();
            var nameComponent = world.GetPool<NameComponent>();
            var timerComponent = world.GetPool<TimerComponent>();
            var adsComponent = world.GetPool<AdsComponent>();
            var unifiedViewKeyComponent = world.GetPool<UnifiedViewKeyComponent>();
            var categoryComponent = world.GetPool<CategoryComponent>();
            var linkPool = world.GetPool<DefaultLink>();
            var rewardPool = world.GetPool<Reward>();

            var entity = world.NewEntity();
            var shopEntity = entity;
            var modelEntity = entity;

            entity = world.NewEntity();
            var categoryEntity = entity;
            linkPool.Add(entity).SetLink(shopEntity);
            categoryComponent.Add(entity) = new CategoryComponent()
            {
                Value = "Best"
            };
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Best"
            };
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(categoryEntity);
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Lot_1"
            };
            timerComponent.Add(entity) = new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            };
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test1";
            
            entity = world.NewEntity();
            var purchaseOptionParentEntity = entity;
            linkPool.Add(entity).SetLink(categoryEntity);
            nameComponent.Add(entity) = new NameComponent()
            {
                Value = "Lot_2"
            };
            timerComponent.Add(entity) = new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            };
            unifiedViewKeyComponent.Add(entity) = new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            };

            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x1";
            rewardPool.Add(entity).count = 1;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x5";
            rewardPool.Add(entity).count = 5;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";
            
            entity = world.NewEntity();
            linkPool.Add(entity).SetLink(purchaseOptionParentEntity);
            nameComponent.Add(entity).Value = "Purchase x10";
            rewardPool.Add(entity).count = 10;
            adsComponent
                .Add(entity)
                .AdsPlacement = "Test2";
        }
    }
}