using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    [System.Serializable]
    public class Currency
    {
        public string name;
        public Sprite icon;
        public int amount;
        public Text currencyText;
        public int conversionRate;
    }

    public List<Currency> currencies = new List<Currency>();

    public void AddCurrency(string currencyName, int amount)
    {
        Currency currency = currencies.Find(c => c.name == currencyName);
        if (currency != null)
        {
            currency.amount += amount;
            UpdateCurrencyText(currency);
        }
    }

    public int GetCurrencyAmount(string currencyName)
    {
        Currency currency = currencies.Find(c => c.name == currencyName);
        return currency != null ? currency.amount : 0;
    }

    private void UpdateCurrencyText(Currency currency)
    {
        if (currency.currencyText != null)
        {
            currency.currencyText.text = currency.amount.ToString();
        }
    }
}
