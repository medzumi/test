using System;
using ApplicationScripts.Ecs;
using ApplicationScripts.Logic.Features.Indexing;
using EcsViewModelPresenting;
using Game.CoreLogic;
using Leopotam.EcsLite;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif
using UnityEngine;


namespace Game
{
    [DefaultExecutionOrder(-1)]
    public class AppRoot : MonoBehaviour
    {
        [SerializeField] private TextAsset _textAsset;
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
                .Add<ViewModelUpdateSystem<InteractComponent>>();
            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }

    [Serializable]
    public struct TestIndexComponent : IIndexComponent<string>
    {
        public string Key;
        
        public string GetIndex()
        {
            return Key;
        }
    }
}