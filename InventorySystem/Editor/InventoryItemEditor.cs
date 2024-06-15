using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(InventoryItem))]
public class InventoryItemEditor : Editor
{
    private InventoryItem item;
    private Dictionary<string, int> currencyAmounts = new Dictionary<string, int>();

    private void OnEnable()
    {
        item = (InventoryItem)target;

        // Initialize currency amounts
        if (item.currencyAmounts != null)
        {
            currencyAmounts = new Dictionary<string, int>(item.currencyAmounts);
        }
        else
        {
            currencyAmounts = new Dictionary<string, int>();
        }

        // Load currencies from the CurrencyManager
        LoadCurrencies();
    }

    private void LoadCurrencies()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CurrencyManager currencyManager = player.GetComponent<CurrencyManager>();
            if (currencyManager != null)
            {
                foreach (var currency in currencyManager.currencies)
                {
                    if (!currencyAmounts.ContainsKey(currency.name))
                    {
                        currencyAmounts[currency.name] = 0;
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Item Settings", EditorStyles.boldLabel);
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.itemDescription = EditorGUILayout.TextField("Item Description", item.itemDescription);

        item.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);
        if (item.itemType != ItemType.Currency)
        {
            item.itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", item.itemIcon, typeof(Sprite), false);
            item.isStackable = EditorGUILayout.Toggle("Is Stackable", item.isStackable);
            if (item.isStackable)
            {
                item.maxStackSize = EditorGUILayout.IntField("Max Stack Size", item.maxStackSize);
            }
        }

        if (item.itemType == ItemType.Consumable)
        {
            item.amount = EditorGUILayout.IntField("Amount", item.amount);
        }
        else if (item.itemType == ItemType.Equipment)
        {
            item.equipmentCategory = (EquipmentCategory)EditorGUILayout.EnumPopup("Equipment Category", item.equipmentCategory);
            if (item.equipmentCategory == EquipmentCategory.Weapon)
            {
                item.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", item.weaponType);
                if (item.weaponType == WeaponType.OneHand)
                {
                    item.isMainHand = EditorGUILayout.Toggle("Main Hand", item.isMainHand);
                    item.isOffHand = EditorGUILayout.Toggle("Off Hand", item.isOffHand);
                }
            }
        }
        else if (item.itemType == ItemType.Currency)
        {
            GUILayout.Label("Currency Amounts", EditorStyles.boldLabel);
            List<string> keys = new List<string>(currencyAmounts.Keys);
            foreach (var key in keys)
            {
                currencyAmounts[key] = EditorGUILayout.IntField(key, currencyAmounts[key]);
            }
        }

        if (item.itemType != ItemType.Currency)
        {
            GUILayout.Label("Item Stats", EditorStyles.boldLabel);
            for (int i = 0; i < item.stats.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                item.stats[i].statType = (StatType)EditorGUILayout.EnumPopup("Stat Type", item.stats[i].statType);
                item.stats[i].value = EditorGUILayout.IntField("Value", item.stats[i].value);
                if (GUILayout.Button("Remove"))
                {
                    item.stats.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Stat"))
            {
                item.stats.Add(new ItemStat());
            }
        }

        if (GUILayout.Button("Save Item"))
        {
            SaveItem();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SaveItem()
    {
        item.currencyAmounts = new Dictionary<string, int>(currencyAmounts);
        EditorUtility.SetDirty(item);
        AssetDatabase.SaveAssets();
        Debug.Log("Item saved successfully.");
    }
}
