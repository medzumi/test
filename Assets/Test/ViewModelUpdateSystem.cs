using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using Game.CoreLogic;
using Leopotam.EcsLite;
using Presenting;
using ViewModel;

namespace EcsViewModelPresenting
{
    public class ViewModelUpdateSystem<TComponent> : EcsSystemBase
        where TComponent : struct
    {
        private collector _collector;
        private EcsPool<TComponent> _componentPool;
        private EcsPool<ListComponent<IEcsPresenter<TComponent>>> _componentBindDataPool;

        public override void PreInit(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _collector = world.Filter<TComponent>().Inc<ListComponent<IEcsPresenter<TComponent>>>()
                .EndCollector(CollectorEvent.Added | CollectorEvent.Dirt);
            _componentPool = world.GetPool<TComponent>();
            _componentBindDataPool = world.GetPool<ListComponent<IEcsPresenter<TComponent>>>();
        }

        public override void Run(EcsSystems systems)
        {
            foreach (var entity in _collector)
            {
                var componentData = _componentPool.Read(entity);
                foreach (var ecsPresenter in _componentBindDataPool.Read(entity).ComponentData)
                {
                    ecsPresenter.Update(componentData);
                }
            }
            _collector.Clear();
        }
    }
}