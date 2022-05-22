namespace Game.CoreLogic
{
    public struct RealValuePriceComponent : IPriceComponent
    {
        public decimal Price;
        public string Bundle;
        
        public string GetCurrencyName()
        {
            return "$";
        }

        public string GetPriceString()
        {
            return $"{Price} $";
        }

        public decimal GetPrice()
        {
            return Price;
        }
    }
}