using Newtonsoft.Json.Linq;

namespace Game.CoreLogic
{
    public interface IConverter
    {
        T Convert<T>(JToken jObject);
    }
}