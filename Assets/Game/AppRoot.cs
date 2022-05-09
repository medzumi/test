using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using Game.CoreLogic;
using Game.PresenterLogic;
using Leopotam.EcsLite;
using presenting.ecslite;
using Unity;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif
using UnityEngine;
using unityPresenting.Unity;
using ViewModel;


namespace Game
{
    [DefaultExecutionOrder(-1)]
    public class AppRoot : MonoBehaviour
    {
        public TestSystem _testSystem;
        private EcsSystems _systems;
        
        private void Awake()
        {
            RegisterContainer();    
        }
        
        private void RegisterContainer()
        {
            _systems = new EcsSystems(new EcsWorld());
#if UNITY_EDITOR
            _systems.Add(new EcsWorldDebugSystem());
#endif
            _systems
                .Add<ViewModelUpdateSystem<MoneyComponent>>()
                .Add<ViewModelUpdateSystem<InteractComponent>>()
                .Add<ViewModelUpdateSystem<ContainerComponent>>()
                .Add<ViewModelUpdateSystem<NameComponent>>()
                .Add<ViewModelUpdateSystem<PurchaseCounterComponent>>()
                .Add<ViewModelUpdateSystem<UnifiedViewKeyComponent>>()
                .Add<ViewModelUpdateSystem<CategoryComponent>>()
                .Add<ViewModelUpdateSystem<HardValuePriceComponent>>()
                .Add<ViewModelUpdateSystem<TimerComponent>>();
            _systems.Add(_testSystem);
            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }

    [Serializable]
    public class TestSystem : EcsSystemBase
    {
        [SerializeField] private MonoViewModel _monoViewModel;

        [PresenterKeyProperty(typeof(EcsPresenterData), typeof(IViewModel))] [SerializeField]
        private string _presenterKey;
        
        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            var world = systems.GetWorld();
            var hardValuePricePool = world.GetPool<HardValuePriceComponent>();
            var nameComponent = world.GetPool<NameComponent>();
            var purchaseCounterComponent = world.GetPool<PurchaseCounterComponent>();
            var timerComponent = world.GetPool<TimerComponent>();
            var unifiedViewKeyComponent = world.GetPool<UnifiedViewKeyComponent>();
            var containerComponentPool = world.GetPool<ContainerComponent>();
            var categoryComponent = world.GetPool<CategoryComponent>();

            var entity = world.NewEntity();
            var modelEntity = entity;
            var containerComponent = new ContainerComponent()
            {
                List = new List<int>()
            };
            containerComponentPool.Add(entity, containerComponent);

            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            var categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_1"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_2"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            entity = world.NewEntity();
            containerComponent.List.Add(entity);
            categoryComponent.Add(entity, new CategoryComponent()
            {
                Value = "Best"
            });
            categoryContainer = new ContainerComponent()
            {
                List = new List<int>()
            };
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Best_2"
            });

            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_5"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Gold",
                Price = 100
            });
            unifiedViewKeyComponent.Add(entity, new UnifiedViewKeyComponent()
            {
                Value = "Lootbox"
            });
            
            entity = world.NewEntity();
            categoryContainer.List.Add(entity);
            nameComponent.Add(entity, new NameComponent()
            {
                Value = "Lot_6"
            });
            timerComponent.Add(entity, new TimerComponent()
            {
                TimerValue = new TimeSpan(1, 1, 1, 1)
            });
            purchaseCounterComponent.Add(entity, new PurchaseCounterComponent()
            {
                Count = 5
            });
            hardValuePricePool.Add(entity, new HardValuePriceComponent()
            {
                CurrencyName = "Saphire",
                Price = 100
            });
            
            PresenterSettings.instance.PresenterResolver.Resolve<EcsPresenterData, IViewModel>(_presenterKey)
                .Initialize(new EcsPresenterData()
                {
                    ModelEntity = modelEntity,
                    ModelWorld = world,
                }, _monoViewModel);
        }
    }
}