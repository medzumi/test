using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utilities.GenericPatterns;

namespace Game.CoreLogic
{
    [DefaultExecutionOrder(-2)]
    public class MonoPresenterResolver : MonoBehaviour, IPresenterResolver
    {
        private class DeserializeObject
        {
            public string Key;
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
            Add<LinkPresenter<TradeComponent>>();
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
                    _buffer.Clear();
                    foreach (var keyValuePair in jObject)
                    {
                        if (_converters.TryGetValue(keyValuePair.Key, out var converter))
                        {
                            _buffer.Add(converter.Convert<IEcsPresenter>(keyValuePair.Value));
                        }
                    }

                    _ecsPresenters[key] = presenter = new AggregatePresenter(_buffer);
                }
                else
                {
                    return null;
                }
            }

            return presenter;
        }
    }
}