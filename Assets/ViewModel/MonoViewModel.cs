using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ApplicationScripts.CodeExtensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utilities.SerializeReferencing;

namespace ViewModel
{
    [DefaultExecutionOrder(-100)]
    public class MonoViewModel : MonoBehaviour, IViewModel
    {
        [Serializable]
        private struct CustomPair
        {
            public string Key;
            [SerializeReference][SerializeTypes(typeof(IViewModelData))] public IViewModelData Data;
        }

        [SerializeField] private List<CustomPair> _customPairs = new List<CustomPair>();

        private void Awake()
        {
            OnValidate();
            _customPairs.Clear();
            _customPairs = null;
        }

        #if UNITY_EDITOR
        private Dictionary<int, IViewModelData> _viewModelDatas = new Dictionary<int, IViewModelData>();
        
        private void OnValidate()
        {
            _viewModelDatas.Clear();
            foreach (var customPair in _customPairs)
            {
                PropertyName propertyName = customPair.Key;
                _viewModelDatas.Add(propertyName.GetHashCode(), customPair.Data);
            }                
        }
        #endif
        public T GetViewModelData<T>(string key) where  T : IViewModelData
        {
            return (T)GetViewModelDataHandler(key);
        }

        public object GetViewModelData(string key)
        {
            return GetViewModelDataHandler(key);
        }

        IDisposable IViewModel.OnDispose(Action<IViewModel> action)
        {
            return gameObject.OnDisableAsObservable()
                .Subscribe(t => action?.Invoke(this));
        }

        private void OnDisable()
        {
            foreach (var keyValuePair in _viewModelDatas)
            {
                if (keyValuePair.Value is IViewModelProperty viewModelProperty)
                {
                    viewModelProperty.Reset();
                }
            }
        }

        private void AddViewModelDataHandler(string key, IViewModelData data)
        {
            if (GetViewModelDataHandler(key) == null)
            {
                _customPairs.Add(new CustomPair()
                {
                    Key =  key,
                    Data = data
                });
            }
        }

        private object GetViewModelDataHandler(string key)
        {
            return _customPairs.FirstOrDefault(pair => string.Equals(key, pair.Key));
        }
    }
}