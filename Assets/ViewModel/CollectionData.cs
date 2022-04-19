using System;
using System.Collections;
using System.Collections.Generic;

namespace ViewModel
{
    [Serializable]
    public class CollectionData : IViewModelData
    {
        public event Action OnUpdate;
        
        private Action<int, IViewModel> _cocnreteFillAction;
        private ICollection _collectionReference = null;

        public int Count => _collectionReference != null ? _collectionReference.Count : 0;

        public void Fill<TCollection, TData>(TCollection collection, Action<TData, IViewModel> fillAction, bool forceUpdate = false)
            where TCollection : class, IList<TData>, ICollection
        {
            //ToDo Fix ForceUpdate
            if (!object.ReferenceEquals(collection, _collectionReference) || forceUpdate)
            {
                _collectionReference = collection;
                    //ToDo : if need present different compoents
                _cocnreteFillAction = (i, model) =>
                {
                    fillAction?.Invoke(collection[i], model);
                };
                OnUpdate?.Invoke();
            }
        }

        public void FillViewModel(int id, IViewModel viewModel)
        {
            _cocnreteFillAction?.Invoke(id, viewModel);
        }
    }
}