using System;
using System.Threading;
using UnityEngine;

namespace ViewModel
{
    public interface IViewModel
    {
        T GetViewModelData<T>(string propertyName) where T : IViewModelData;

        object GetViewModelData(string propertyName);

        IDisposable OnDispose(Action<IViewModel> action);
    }
}