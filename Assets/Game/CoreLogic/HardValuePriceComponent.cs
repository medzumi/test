using System;
using UnityEngine.Purchasing;

namespace Game.CoreLogic
{
    [Serializable]
    public struct HardValuePriceComponent : IPriceComponent
    {
        public string CurrencyName;
        public decimal Price;

        public string GetCurrencyName()
        {
            return CurrencyName;
        }

        public string GetPriceString()
        {
            return $"{Price} {CurrencyName}";
        }

        public decimal GetPrice()
        {
            return Price;
        }
    }

    public class InAppPriceComponent : IPriceComponent
    {
        public Product Product;
        
        public string GetCurrencyName()
        {
            return Product?.metadata.isoCurrencyCode;
        }

        public string GetPriceString()
        {
            return Product?.metadata.localizedPriceString;
        }

        public decimal GetPrice()
        {
            return Product?.metadata.localizedPrice ?? 0;
        }
    }

    public class AdsComponent
    {
        
    }

    public interface IPriceComponent
    {
        public string GetCurrencyName();
        public string GetPriceString();
        public decimal GetPrice();
    }
}