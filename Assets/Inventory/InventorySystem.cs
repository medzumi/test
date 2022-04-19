using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Logic.Features.Indexing;
using Leopotam.EcsLite;

namespace Inventory
{
    public class InventorySystem : EcsSystemBase
    {
        private readonly string _workWorld;
        private readonly string _staticWorld;

        private EcsPool<InventoryData> _inventories;
        private EcsPool<AddItem> _addItemCommands;
        private EcsPool<RemoveItem> _removeItemCommands;
        private EcsPool<InventoryIndex> _inventoryIndexPool;
        private EcsPool<Stackable> _stackablePool;

        private EcsFilter _addFilter;
        private EcsFilter _removeFilter;

        private IndexedEntityLibrarySystem<InventoryIndex, int> _indexedEntityLibrary;
        
        private EcsWorld _staticEcsWorld;

        //ToDo pool for multithread
        private List<FindInventoryData> _buffer = new List<FindInventoryData>();

        public InventorySystem(string workWorld, string staticWorld)
        {
            _workWorld = workWorld;
            _staticWorld = staticWorld;
        }

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetSafeWorld(_workWorld);
            _stackablePool = world.GetPool<Stackable>();
            _staticEcsWorld = systems.GetSafeWorld(_staticWorld);
            _inventories = world.GetPool<InventoryData>();
            _inventoryIndexPool = world.GetPool<InventoryIndex>();
            _addItemCommands = world.GetPool<AddItem>();
            _removeItemCommands = world.GetPool<RemoveItem>();
            IndexedEntityLibrarySystem<InventoryIndex, int>.GetLibrary(world);
        }

        public override void Run(EcsSystems systems)
        {
            AddOperations();
            RemoveOperations();
        }

        private void AddOperations()
        {
            foreach (var addEntity in _addFilter)
            {
                ref var addCommand = ref _addItemCommands.Set(addEntity);
                var inventory = _inventories.Read(addCommand.InventoryIndex);
                foreach (var findInventoryData in GetInventoryData(inventory, addCommand.ItemIndex, _buffer))
                {
                    if (_stackablePool.Has(findInventoryData.ItemIndex))
                    {
                        ref var stackable = ref _stackablePool.Set(findInventoryData.ItemIndex);
                        var space = stackable.MaxCount - stackable.Count;
                        if (space > 0)
                        {
                            if (space > addCommand.Count)
                            {
                                stackable.Count += addCommand.Count;
                                _addItemCommands.Del(addEntity);
                            }
                            else
                            {
                                stackable.Count = stackable.MaxCount;
                                addCommand.Count -= space;
                            }
                        }
                    }   
                }

                foreach (var findInventoryData in GetEmptyData(inventory, _buffer))
                {
                    inventory.Data[findInventoryData.SlotIndex] = addCommand.ItemIndex;
                }
            }
        }

        private void RemoveOperations()
        {
            foreach (var removeEntity in _removeFilter)
            {
                ref var removeCommand = ref _removeItemCommands.Set(removeEntity);
                var inventory = _inventories.Set(removeCommand.InventoryIndex);
                if (_stackablePool.Has(removeCommand.ItemIndex))
                {
                    foreach (var findInventoryData in GetInventoryData(inventory, removeCommand.ItemIndex, _buffer))
                    {
                        if (_stackablePool.Has(findInventoryData.ItemIndex))
                        {
                            ref var stackable = ref _stackablePool.Set(findInventoryData.ItemIndex);
                            if (stackable.Count > removeCommand.Count)
                            {
                                stackable.Count -= removeCommand.Count;
                                _removeItemCommands.Del(removeEntity);
                            }
                            else
                            {
                                removeCommand.Count -= stackable.Count;
                                _stackablePool.Del(findInventoryData.ItemIndex);
                                inventory.Data[findInventoryData.SlotIndex] = -1;
                            }
                        }
                    }
                }
                else
                {
                    var concreteSlot = GetConcreteSlot(inventory, removeCommand.ItemIndex);
                    if (concreteSlot > -1)
                    {
                        inventory.Data[concreteSlot] = -1;
                    }
                }
            }
        }

        private struct FindInventoryData
        {
            public int SlotIndex;
            public int InventoryIndex;
            public int ItemIndex;
        }

        private int GetConcreteSlot(InventoryData inventoryData, int itemIndex)
        {
            var slotId = 0;
            foreach (var index in inventoryData.Data)
            {
                if (index == itemIndex)
                {
                    return slotId;
                }

                slotId++;
            }

            return -1;
        }
        
        private List<FindInventoryData> GetEmptyData(InventoryData inventoryData, List<FindInventoryData> buffer)
        {
            int slotId = 0;
            foreach (var index in inventoryData.Data)
            {
                if (index == -1)
                {
                    buffer.Add(new FindInventoryData()
                    {
                        SlotIndex = slotId,
                        InventoryIndex = -1,
                        ItemIndex = -1
                    });
                }

                slotId++;
            }

            return buffer;
        }

        private List<FindInventoryData> GetInventoryData(InventoryData inventoryData,
            int inventoryIndex, List<FindInventoryData> buffer)
        {
            int slotId = 0;
            foreach (var index in inventoryData.Data)
            {
                if (index > -1 && _inventoryIndexPool.Has(index))
                {
                    var inventoryIndexComponent = _inventoryIndexPool.Read(index);
                    if (inventoryIndexComponent.Index == inventoryIndex)
                    {
                        buffer.Add(new FindInventoryData()
                        {
                            SlotIndex = slotId,
                            InventoryIndex = inventoryIndex,
                            ItemIndex = index
                        });
                    }
                }

                slotId++;
            }

            return buffer;
        }
    }

    public struct RemoveItem
    {
        public int InventoryIndex;
        public int ItemIndex;
        public int Count;
    }
}