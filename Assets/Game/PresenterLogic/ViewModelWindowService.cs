using System;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;

namespace Game.PresenterLogic
{
    public class ViewModelWindowService : IViewModelWindowService
    {
        private readonly Dictionary<IViewModel, object> _windowsDictionary = new Dictionary<IViewModel, object>();
        
        public void ShowSubWindow<TModel>(string key, TModel model, IViewModel viewModel)
        {
            if (viewModel is MonoBehaviour monoBehaviour)
            {
                var currentViewModelTransform = monoBehaviour.transform;
                object obj = null;
                while (!currentViewModelTransform.TryGetComponent(out IViewModel windowViewModel) || !_windowsDictionary.TryGetValue(windowViewModel, out obj))
                {
                    currentViewModelTransform = currentViewModelTransform.parent;
                }

                if (obj is IWindowsPresenter windowsPresenter)
                {
                    windowsPresenter.OpenWindow(key, model);
                }
            }
            else
            {
                throw new Exception("View model isn't mono behaviour");
            }
        }

        public void UnregisterWindow(IViewModel viewModel)
        {
            _windowsDictionary.Remove(viewModel);
        }

        public void RegisterWindow(IViewModel viewModel, IWindowsPresenter windowsPresenter)
        {
            _windowsDictionary.Add(viewModel, windowsPresenter);
        }
    }
}