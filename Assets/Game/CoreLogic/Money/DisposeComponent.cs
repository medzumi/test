using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using Leopotam.EcsLite;

namespace Game.CoreLogic
{
    public struct DisposeComponent : IEcsAutoReset<DisposeComponent>
    {
        public List<IDisposable> Disposables;

        public void AutoReset(ref DisposeComponent c)
        {
            if (CodeBeautifyExtensions.IsNull<List<IDisposable>>(c.Disposables))
            {
                c.Disposables = new List<IDisposable>();
            }
            else
            {
                foreach (var disposable in c.Disposables)
                {
                    disposable?.Dispose();
                }
                c.Disposables.Clear();
            }
        }
    }
}