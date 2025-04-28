using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CreateItemEditor : InventoryManagerBaseEditor
{
    private string itemName = "New Item";
    private string itemDescription = "Item Description";
    private Sprite itemIcon;
    private bool isStackable;
    private int maxStackSize = 1;
    private ItemType itemType;
    private int amount = 0;  // For consumables like health potions
    private EquipmentCategory equipmentCategory;
    private WeaponType weaponType;
    private bool isMainHand;
    private bool isOffHand;
    private List<ItemStat> itemStats = new List<ItemStat>();
    private Dictionary<string, int> currencyAmounts = new Dictionary<string, int>();

    private InventoryItem item;
    private List<GameObject> itemPrefabs = new List<GameObject>();
    private GameObject pickupTextPrefab;
    private GameObject itemPickupPrefab;
    private float sphereRadius = 1.0f;
    private float sphereHeight = -0.5f;

    private bool setBone;
    private List<string> selectedBoneNames = new List<string>();
    private List<Vector3> itemPositions = new List<Vector3>();
    private List<Vector3> itemRotations = new List<Vector3>();

    public void DrawCreateAndSetupItem()
    {
        GUILayout.Label("Item Settings", EditorStyles.boldLabel);
        itemName = EditorGUILayout.TextField("Item Name", itemName);
        itemDescription = EditorGUILayout.TextField("Item Description", itemDescription);

        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);
        if (itemType != ItemType.Currency)
        {
            itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", itemIcon, typeof(Sprite), false);
            isStackable = EditorGUILayout.Toggle("Is Stackable", isStackable);
            if (isStackable)
            {
                maxStackSize = EditorGUILayout.IntField("Max Stack Size", maxStackSize);
            }
        }

        if (itemType == ItemType.Consumable)
        {
            amount = EditorGUILayout.IntField("Amount", amount);
        }
        else if (itemType == ItemType.Equipment)
        {
            equipmentCategory = (EquipmentCategory)EditorGUILayout.EnumPopup("Equipment Category", equipmentCategory);
            if (equipmentCategory == EquipmentCategory.Weapon)
            {
                weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
                if (weaponType == WeaponType.OneHand)
                {
                    isMainHand = EditorGUILayout.Toggle("Main Hand", isMainHand);
                    isOffHand = EditorGUILayout.Toggle("Off Hand", isOffHand);
                }
            }
        }
        else if (itemType == ItemType.Currency)
        {
            GUILayout.Label("Currency Amounts", EditorStyles.boldLabel);
            List<string> keys = new List<string>(currencyAmounts.Keys);
            foreach (var key in keys)
            {
                if (!currencyAmounts.ContainsKey(key))
                {
                    currencyAmounts[key] = 0;
                }
                currencyAmounts[key] = EditorGUILayout.IntField(key, currencyAmounts[key]);
            }
        }

        if (itemType != ItemType.Currency)
        {
            GUILayout.Label("Item Stats", EditorStyles.boldLabel);
            for (int i = 0; i < itemStats.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                itemStats[i].statType = (StatType)EditorGUILayout.EnumPopup("Stat Type", itemStats[i].statType);
                itemStats[i].value = EditorGUILayout.IntField("Value", itemStats[i].value);
                if (GUILayout.Button("Remove"))
                {
                    itemStats.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Stat"))
            {
                itemStats.Add(new ItemStat());
            }
        }

        GUILayout.Label("Item Setup", EditorStyles.boldLabel);
        itemPickupPrefab = (GameObject)EditorGUILayout.ObjectField("Item Pickup Prefab", itemPickupPrefab, typeof(GameObject), false);
        pickupTextPrefab = (GameObject)EditorGUILayout.ObjectField("Pickup Text Prefab", pickupTextPrefab, typeof(GameObject), false);

        EditorGUILayout.LabelField("Item Prefabs");
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            itemPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Item Prefab {i + 1}", itemPrefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("Remove"))
            {
                itemPrefabs.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Item Prefab"))
        {
            itemPrefabs.Add(null);
        }

        GUILayout.Label("Collider Settings", EditorStyles.boldLabel);
        sphereRadius = EditorGUILayout.FloatField("Sphere Radius", sphereRadius);
        sphereHeight = EditorGUILayout.FloatField("Sphere Height", sphereHeight);

        // New fields for bone setup
        setBone = EditorGUILayout.Toggle("Set Bone", setBone);
        if (setBone)
        {
            EditorGUILayout.LabelField("Bone Settings");
            for (int i = 0; i < selectedBoneNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                selectedBoneNames[i] = EditorGUILayout.TextField($"Bone Name {i + 1}", selectedBoneNames[i]);
                itemPositions[i] = EditorGUILayout.Vector3Field($"Position {i + 1}", itemPositions[i]);
                itemRotations[i] = EditorGUILayout.Vector3Field($"Rotation {i + 1}", itemRotations[i]);
                if (GUILayout.Button("Remove"))
                {
                    selectedBoneNames.RemoveAt(i);
                    itemPositions.RemoveAt(i);
                    itemRotations.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Bone"))
            {
                selectedBoneNames.Add("");
                itemPositions.Add(Vector3.zero);
                itemRotations.Add(Vector3.zero);
            }
        }

        if (GUILayout.Button("Create Item"))
        {
            CreateAndSetupItem();
        }
    }

    private void CreateAndSetupItem()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("Item name is required.");
            return;
        }

        if (itemPickupPrefab == null || pickupTextPrefab == null)
        {
            Debug.LogError("Please assign the Item Pickup Prefab and Pickup Text Prefab.");
            return;
        }

        // Ensure Resources folder exists
        string resourcesPath = "Assets/InventorySystem/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets/InventorySystem", "Resources");
        }

        // Create a new InventoryItem ScriptableObject
        InventoryItem newItem = ScriptableObject.CreateInstance<InventoryItem>();
        newItem.itemName = itemName;
        newItem.itemDescription = itemDescription;
        newItem.itemIcon = itemIcon;
        newItem.isStackable = isStackable;
        newItem.maxStackSize = maxStackSize;
        newItem.itemType = itemType;
        newItem.amount = amount;
        newItem.equipmentCategory = equipmentCategory;
        newItem.weaponType = weaponType;
        newItem.isMainHand = isMainHand;
        newItem.isOffHand = isOffHand;
        newItem.stats = new List<ItemStat>(itemStats);
        newItem.itemPrefabs = new List<GameObject>(itemPrefabs); // Assign the list of item prefabs
        newItem.pickupTextPrefab = pickupTextPrefab;
        newItem.itemPickupPrefab = itemPickupPrefab;

        // Set bone attachment details if applicable
        if (setBone)
        {
            newItem.setBone = true;
            newItem.selectedBoneNames = new List<string>(selectedBoneNames);
            newItem.itemPositions = new List<Vector3>(itemPositions);
            newItem.itemRotations = new List<Vector3>(itemRotations);
        }

        // Set currency amounts if item type is Currency
        if (itemType == ItemType.Currency)
        {
            newItem.currencyAmounts = new Dictionary<string, int>(currencyAmounts);
        }

        // Save the new InventoryItem ScriptableObject to the Resources folder
        string itemPath = resourcesPath + "/" + itemName + ".asset";
        AssetDatabase.CreateAsset(newItem, itemPath);
        AssetDatabase.SaveAssets();

        // Setup the item prefab
        SetupItem(newItem);

        Debug.Log("Item created and setup successfully.");
    }

    private void SetupItem(InventoryItem newItem)
    {
        string resourcesPath = "Assets/InventorySystem/Resources";
        string itemPath = resourcesPath + "/" + newItem.itemName + ".prefab";
        GameObject itemInstance = Instantiate(newItem.itemPickupPrefab); // Use the pickup prefab from the item
        itemInstance.name = newItem.itemName;

        SphereCollider collider = itemInstance.AddComponent<SphereCollider>();
        collider.radius = sphereRadius;
        collider.center = new Vector3(0, sphereHeight, 0);
        collider.isTrigger = true;

        if (newItem.itemType == ItemType.Currency)
        {
            CurrencyPickup currencyPickup = itemInstance.AddComponent<CurrencyPickup>();
            currencyPickup.pickupTextPrefab = pickupTextPrefab;
            foreach (var currency in newItem.currencyAmounts)
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
            itemPickup.item = newItem;
            itemPickup.pickupTextPrefab = pickupTextPrefab;
        }

        // Save the new prefab to the Resources folder
        PrefabUtility.SaveAsPrefabAsset(itemInstance, itemPath);
        DestroyImmediate(itemInstance);
    }
}
