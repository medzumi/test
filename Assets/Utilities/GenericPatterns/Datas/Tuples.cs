using System;

namespace Utilities.GenericPatterns.Datas
{
    [Serializable]
    public class Tuple<T1, T2>
    {
        public T1 FirstElement;
        public T2 SecondElement;
    }

    [Serializable]
    public class Tuple<T1, T2, T3>
    {
        public T1 FirstElement;
        public T2 SecondElement;
        public T3 ThirdElement;
    }

    [Serializable]
    public struct ValueTuple<T1, T2>
    {
        public T1 FirstElement;
        public T2 SecondElement;
    }

    [Serializable]
    public struct ValueTuple<T1, T2, T3>
    {
        public T1 FirstElement;
        public T2 SecondElement;
        public T3 ThirdElement;
    }
}