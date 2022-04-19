namespace Utilities.GenericPatterns
{
    public class Singletone<T> where T : new()
    {
        private static T _instance;
        
        public static T instance
        {
            get
            {
                return _instance ??= new T();
            }
        }
    }
}