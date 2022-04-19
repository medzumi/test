using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Client
{
    public class InventorySlotPresenter : EcsUguiActionBase<InventorySlotClick>
    {
        [SerializeField] private Button _button;
    }

    public struct InventorySlotClick
    {
        
    }
}