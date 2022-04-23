using System;
using System.Collections.Generic;

namespace Game.CoreLogic
{
    [Serializable]
    public class LinkPresenter<TData> : AbstractEcsPresenter<LinkPresenter<TData>, TData>
        where TData : ILinkableComponent
    {
        public List<string> _binders = new List<string>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public override void Update(TData data)
        {
            base.Update(data);
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            CompareLists();
            int index = 0;
            foreach (var entity in data.GetLinks())
            {
                var presenter = Resolve(_binders[index]);
                presenter.Initialize(new EcsPresenterData()
                {
                    ModelEntity = entity,
                    ModelWorld = EcsPresenterData.ModelWorld,
                    ViewModel = EcsPresenterData.ViewModel
                });
                _disposables[index] = presenter;
                index++;
            }
        }

        private void CompareLists()
        {
            for (int i = _disposables.Count; i < _binders.Count; i++)
            {
                _disposables.Add(null);
            }
        }

        protected override LinkPresenter<TData> CloneHandler()
        {
            var clone = base.CloneHandler();
            clone._binders.AddRange(_binders);
            return clone;
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            _binders.Clear();
        }

        public string ComponentName => $"LinkPresenter<{typeof(TData).Name}>";
    }
}