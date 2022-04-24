using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utilities.GenericPatterns;
using ViewModel;

namespace Game.CoreLogic
{
    [DefaultExecutionOrder(-2)]
    public class MonoPresenterResolver : MonoBehaviour, IPresenterResolver
    {
        private class DeserializeObject
        {
            public string Key;
            public string ViewModelPath;
            public JObject JObject;
        }
        
        [SerializeField] private TextAsset _textAsset;

        private readonly Dictionary<string, IEcsPresenter> _ecsPresenters = new Dictionary<string, IEcsPresenter>();
        private readonly Dictionary<string, IConverter> _converters = new Dictionary<string, IConverter>();
        private List<DeserializeObject> _jObjects;
        private List<IEcsPresenter> _buffer = new List<IEcsPresenter>();

        private void Awake()
        {
            _jObjects = JsonConvert.DeserializeObject<List<DeserializeObject>>(_textAsset.text);
            Add<MoneyPresenter>();
            Add<LinkPresenter<InteractComponent>>();
            Singletone<IPresenterResolver>.instance = this;
        }

        private Dictionary<string, IConverter> Add<T>()
        {
            _converters.Add(Converter<T>.typeKey, new Converter<T>());
            return _converters;
        }

        public IEcsPresenter Resolve(string key)
        {
            if (!_ecsPresenters.TryGetValue(key, out var presenter))
            {
                var jObject = _jObjects.FirstOrDefault(deserialize => string.Equals(deserialize.Key, key))?.JObject;
                if (jObject != null)
                {
                    _ecsPresenters[key] = presenter = Resolve(jObject);
                }
                else
                {
                    return null;
                }
            }

            return presenter.Clone();
        }

        protected IEcsPresenter Resolve(JObject jObject)
        {
            _buffer.Clear();
            foreach (var keyValuePair in jObject)
            {
                if (_converters.TryGetValue(keyValuePair.Key, out var converter))
                {
                    _buffer.Add(converter.Convert<IEcsPresenter>(keyValuePair.Value));
                }
            }

            return new AggregatePresenter(_buffer);
        }

        private DeserializeObject GetDeserializeObject(string key)
        {
            return _jObjects.FirstOrDefault(deserialize => string.Equals(deserialize.Key, key));
        }

        public void BindPresenter(RequestData requestData)
        {
            var deserializeObject = GetDeserializeObject(requestData.Key);
            var viewModel = Resources.Load<MonoViewModel>(deserializeObject.ViewModelPath);
            if (!_ecsPresenters.TryGetValue(requestData.Key, out var presenter))
            {
                presenter = _ecsPresenters[requestData.Key] = Resolve(deserializeObject.JObject);
            }
            presenter.Clone().Initialize(new PresenterData()
            {
                ModelEntity = requestData.ModelEntity,
                ModelWorld = requestData.ModelWorld,
                ViewModel = viewModel
            });
        }
    }
}