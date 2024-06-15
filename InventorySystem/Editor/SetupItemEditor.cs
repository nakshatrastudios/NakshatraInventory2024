using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SetupItemEditor : EditorWindow
{
    private InventoryItem item;
    private GameObject itemPrefab;
    private GameObject pickupTextPrefab;
    private Vector3 colliderSize = Vector3.one;
    private float sphereRadius = 1.0f;
    private float sphereHeight = -0.5f;

    private List<CurrencyManager.Currency> currencies;
    private Dictionary<string, int> currencyAmounts = new Dictionary<string, int>();

    [MenuItem("Inventory System/Setup Item")]
    public static void ShowWindow()
    {
        GetWindow<SetupItemEditor>("Setup Item");
    }

    private void OnEnable()
    {
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
                currencies = currencyManager.currencies;
                foreach (var currency in currencies)
                {
                    currencyAmounts[currency.name] = 0;
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Setup", EditorStyles.boldLabel);
        item = (InventoryItem)EditorGUILayout.ObjectField("Item", item, typeof(InventoryItem), false);
        itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", itemPrefab, typeof(GameObject), false);
        pickupTextPrefab = (GameObject)EditorGUILayout.ObjectField("Pickup Text Prefab", pickupTextPrefab, typeof(GameObject), false);

        GUILayout.Label("Collider Settings", EditorStyles.boldLabel);
        sphereRadius = EditorGUILayout.FloatField("Sphere Radius", sphereRadius);
        sphereHeight = EditorGUILayout.FloatField("Sphere Height", sphereHeight);

        if (item != null && item.itemType == ItemType.Currency)
        {
            GUILayout.Label("Currency Amounts", EditorStyles.boldLabel);
            foreach (var currency in currencies)
            {
                currencyAmounts[currency.name] = EditorGUILayout.IntField(currency.name, currencyAmounts[currency.name]);
            }
        }

        if (GUILayout.Button("Setup Item"))
        {
            SetupItem();
        }
    }

    private void SetupItem()
    {
        if (item == null || itemPrefab == null || pickupTextPrefab == null)
        {
            Debug.LogError("Please assign all required fields.");
            return;
        }

        // Ensure Resources folder exists
        string resourcesPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        string itemPath = resourcesPath + "/" + item.itemName + ".prefab";
        GameObject itemInstance = Instantiate(itemPrefab);
        itemInstance.name = item.itemName;

        SphereCollider collider = itemInstance.AddComponent<SphereCollider>();
        collider.radius = sphereRadius;
        collider.center = new Vector3(0, sphereHeight, 0);
        collider.isTrigger = true;

        if (item.itemType == ItemType.Currency)
        {
            CurrencyPickup currencyPickup = itemInstance.AddComponent<CurrencyPickup>();
            currencyPickup.pickupTextPrefab = pickupTextPrefab;
            foreach (var currency in currencyAmounts)
            {
                currencyPickup.currencyAmounts.Add(new CurrencyPickup.CurrencyAmount
                {
                    name = currency.Key,
                    amount = currency.Value
                });
            }
        }
        else
        {
            ItemPickup itemPickup = itemInstance.AddComponent<ItemPickup>();
            itemPickup.item = item;
            itemPickup.pickupTextPrefab = pickupTextPrefab;
        }

        // Save the new prefab to the Resources folder
        PrefabUtility.SaveAsPrefabAsset(itemInstance, itemPath);
        DestroyImmediate(itemInstance);

        Debug.Log("Item setup successfully.");
    }
}
