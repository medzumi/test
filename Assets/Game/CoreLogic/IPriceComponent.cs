namespace Game.CoreLogic
{
    public interface IPriceComponent
    {
        public string GetCurrencyName();
        public string GetPriceString();
        public decimal GetPrice();
    }
}