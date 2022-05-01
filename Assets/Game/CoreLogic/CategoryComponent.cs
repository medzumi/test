using Packages.ecslite.extensions.CommonComponents;

namespace Game.CoreLogic
{
    public struct CategoryComponent : ISingleValueCompnent<string>
    {
        public string Value;
        
        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Value = value;
        }
    }
}