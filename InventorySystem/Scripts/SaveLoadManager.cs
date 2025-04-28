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

    public void SaveGame(Inventory inventory, Equipment equipment, QuickAccessBar quickAccessBar)
    {
        GameSaveData saveData = new GameSaveData
        {
            inventoryItems = inventory.GetItems(),
            equipmentItems = equipment.GetItems(),
            quickAccessItems = quickAccessBar.GetItems()
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }

    public void LoadGame(Inventory inventory, Equipment equipment, QuickAccessBar quickAccessBar)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        // Clear existing items before loading new ones
        equipment.ClearItems();
        inventory.ClearItems();
        quickAccessBar.ClearItems();

        // Update allItemsList from itemDB before loading items
        inventory.PopulateAllItemsList();
        equipment.PopulateAllItemsList();
        quickAccessBar.PopulateAllItemsList();

        inventory.LoadItems(saveData.inventoryItems);
        equipment.LoadItems(saveData.equipmentItems);
        quickAccessBar.LoadItems(saveData.quickAccessItems);

        Debug.Log("Game loaded from " + savePath);
    }
}

[System.Serializable]
public class GameSaveData
{
    public List<InventoryItemData> inventoryItems;
    public List<InventoryItemData> equipmentItems;
    public List<InventoryItemData> quickAccessItems;
}


[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int quantity;
}
