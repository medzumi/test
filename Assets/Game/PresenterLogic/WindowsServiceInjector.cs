using presenting.Unity.Default;
using UnityEngine;
using unityPresenting.Unity;

namespace Game.PresenterLogic
{
    [CreateAssetMenu]
    public class WindowsServiceInjector : AbstractInjector
    {
        private static ViewModelWindowService _service;
        private static IViewModelWindowService Service => _service ??= new ViewModelWindowService();
        
        public override void Inject(object obj, string key)
        {
            Inject<object>(obj, key);
        }

        public override void Inject<TObject>(TObject obj, string key)
        {
            if (obj is IInject<IViewModelWindowService> inject)
            {
                inject.Inject(Service);
            }
        }
    }
}