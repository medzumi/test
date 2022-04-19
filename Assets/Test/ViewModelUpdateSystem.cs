using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using Game.CoreLogic;
using Leopotam.EcsLite;
using Presenting;
using ViewModel;

namespace EcsViewModelPresenting
{
    public class ViewModelUpdateSystem<TComponent, TComponentBindData> : EcsSystemBase
        where TComponent : struct
        where TComponentBindData : struct, IEcsPresenter<TComponent>, IBindData
    {
        private collector _collector;
        private EcsPool<TComponent> _componentPool;
        private EcsPool<TComponentBindData> _componentBindDataPool;

        public override void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _collector = world.Filter<TComponent>().Inc<TComponentBindData>()
                .EndCollector(CollectorEvent.Added | CollectorEvent.Dirt);
            _componentPool = world.GetPool<TComponent>();
            _componentBindDataPool = world.GetPool<TComponentBindData>();
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _collector)
            {
                _componentBindDataPool.Read(entity)
                    .Update(_componentPool.Read(entity));
            }
            _collector.Clear();
        }
    }
}