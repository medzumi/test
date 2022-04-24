using System;
using Newtonsoft.Json.Linq;

namespace Game.CoreLogic
{
    public class Converter<T> : IConverter
    {
        public static readonly string typeKey;

        static Converter()
        {
            var typeName = typeof(T);
            typeKey = GetNameWithoutNameSpaceAndAssemblies(typeof(T));
        }

        private static string GetNameWithoutNameSpaceAndAssemblies(Type type)
        {
            if (type.IsGenericType)
            {
                var typeName = $"{type.Name}<";
                var genericTypes = type.GetGenericArguments();
                for (int i = 0; i < genericTypes.Length; i++)
                {
                    typeName += GetNameWithoutNameSpaceAndAssemblies(genericTypes[i]);
                    if (i < genericTypes.Length - 1)
                    {
                        typeName += ",";
                    }
                }

                typeName += '>';
                return typeName;
            }
            else
            {
                return type.Name;
            }
        }

        public T1 Convert<T1>(JToken jObject)
        {
            if (typeof(T1).IsAssignableFrom(typeof(T)))
            {
                return (T1)(object)jObject.ToObject<T>();
            }

            return default;
        }
    }
}