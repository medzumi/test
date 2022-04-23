using System.Collections.Generic;

namespace Game.CoreLogic
{
    public sealed class AggregatePresenter : AbstractEcsPresenter<AggregatePresenter>
    {
        private List<IEcsPresenter> _presenters;

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
            foreach (var ecsPresenter in _presenters)
            {
                ecsPresenter.Initialize(ecsPresenterData);
            }
        }

        public AggregatePresenter() : base()
        {
            _presenters = new List<IEcsPresenter>();
        }

        public AggregatePresenter(List<IEcsPresenter> presenters) : this()
        {
            _presenters.AddRange(presenters);
        }

        protected override void DisposeHandler()
        {
            base.DisposeHandler();
            foreach (var ecsPresenter in _presenters)
            {
                ecsPresenter.Dispose();
            }
            _presenters.Clear();
        }

        protected override AggregatePresenter CloneHandler()
        {
            var clone = base.CloneHandler();
            foreach (var ecsPresenter in _presenters)
            {
                clone._presenters.Add(ecsPresenter.Clone());
            }

            return clone;
        }
    }
}