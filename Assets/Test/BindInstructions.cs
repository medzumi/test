using System;
using System.Collections.Generic;
using Injecting;
using Leopotam.EcsLite;
using Presenting;
using UnityEngine;
using Utilities.SerializeReferencing;
using ViewModel;

namespace EcsViewModelPresenting
{
    [CreateAssetMenu]
    public class BindInstructions : ScriptableInjectInstructions
    {
        [Serializable]
        private class Instruction
        {
            public string Key;

            [SerializeReference] [SerializeTypes(typeof(IBindData))]
            public object[] Binder = new object[0];

            public GameObject GameObject;
        }

        [SerializeField] private List<Instruction> _instructions;

        public override void SetInstructions(Container container)
        {
        }
    }

    public interface IViewModelBinder
    {
        void Bind(IViewModel viewModel, EcsWorld ecsWorld, int entity);
    }
}
