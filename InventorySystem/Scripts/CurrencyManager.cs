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

    private void Start()
    {
        foreach (var currency in currencies)
        {
            if (currency.currencyText != null)
            {
                currency.currencyText.text = currency.amount.ToString();
            }
            else
            {
                Debug.LogError($"Currency text for '{currency.name}' is not assigned.");
            }
        }
    }

    public void AddCurrency(string currencyName, int amount)
    {
        Currency currency = currencies.Find(c => c.name == currencyName);
        if (currency != null)
        {
            if (currency.conversionRate <= 0)
            {
                Debug.LogError($"Conversion rate for '{currency.name}' must be greater than 0.");
                return;
            }

            // Calculate the new total amount including the newly added amount
            int totalAmount = currency.amount + amount;

            // Determine the amount that can be converted to the next higher currency
            int convertedAmount = totalAmount / currency.conversionRate;

            // Determine the remaining amount of the current currency
            int remainder = totalAmount % currency.conversionRate;

            // Update the current currency's amount
            currency.amount = remainder;
            Debug.Log($"{currency.name} updated amount: {currency.amount}");

            // Update the currency text if assigned
            UpdateCurrencyText(currency);

            // If there is a converted amount and a higher tier currency exists, add the converted amount to the higher tier currency
            int currentIndex = currencies.IndexOf(currency);
            if (convertedAmount > 0 && currentIndex > 0)
            {
                Debug.Log($"Converted {amount} {currencyName} into {convertedAmount} {currencies[currentIndex - 1].name}");
                AddCurrency(currencies[currentIndex - 1].name, convertedAmount);
            }
        }
        else
        {
            Debug.LogError($"Currency '{currencyName}' not found.");
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
        else
        {
            Debug.LogError($"Currency text for '{currency.name}' is not assigned.");
        }
    }
}
