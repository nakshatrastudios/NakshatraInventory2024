using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/game_save.json";
    }

    /// <summary>
    /// Save inventory, equipment, quick‐access and currency data all at once.
    /// </summary>
    public void SaveGame(
        Inventory inventory,
        Equipment equipment,
        QuickAccessBar quickAccessBar,
        CurrencyManager currencyManager)
    {
        var saveData = new GameSaveData
        {
            inventoryItems   = inventory.GetItems(),
            equipmentItems   = equipment.GetItems(),
            quickAccessItems = quickAccessBar.GetItems(),
            currencyAmounts  = currencyManager.GetCurrencyData()
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }

    /// <summary>
    /// Load inventory, equipment, quick‐access and currency data from disk.
    /// </summary>
    public void LoadGame(
        Inventory inventory,
        Equipment equipment,
        QuickAccessBar quickAccessBar,
        CurrencyManager currencyManager)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        var saveData = JsonUtility.FromJson<GameSaveData>(json);

        // Clear existing
        equipment.ClearItems();
        inventory.ClearItems();
        quickAccessBar.ClearItems();

        // Rebuild lists from your ItemDB if needed
        inventory.PopulateAllItemsList();
        equipment.PopulateAllItemsList();
        quickAccessBar.PopulateAllItemsList();

        // Load the items & currency
        inventory.LoadItems(saveData.inventoryItems);
        equipment.LoadItems(saveData.equipmentItems);
        quickAccessBar.LoadItems(saveData.quickAccessItems);
        currencyManager.SetCurrencyData(saveData.currencyAmounts);

        Debug.Log("Game loaded from " + savePath);
    }
}

[System.Serializable]
public class GameSaveData
{
    public List<InventoryItemData> inventoryItems;
    public List<InventoryItemData> equipmentItems;
    public List<InventoryItemData> quickAccessItems;

    // Reuses the CurrencyData you defined in CurrencyManager.cs
    public List<CurrencyData> currencyAmounts;
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int    quantity;
}
