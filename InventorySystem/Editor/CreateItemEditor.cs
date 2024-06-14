using UnityEditor;
using UnityEngine;

public class CreateItemEditor : EditorWindow
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

    [MenuItem("Inventory System/Create Item")]
    public static void ShowWindow()
    {
        GetWindow<CreateItemEditor>("Create Item");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Settings", EditorStyles.boldLabel);
        itemName = EditorGUILayout.TextField("Item Name", itemName);
        itemDescription = EditorGUILayout.TextField("Item Description", itemDescription);
        itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", itemIcon, typeof(Sprite), false);
        isStackable = EditorGUILayout.Toggle("Is Stackable", isStackable);
        if (isStackable)
        {
            maxStackSize = EditorGUILayout.IntField("Max Stack Size", maxStackSize);
        }

        itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);
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

        if (GUILayout.Button("Create Item"))
        {
            CreateItem();
        }
    }

    private void CreateItem()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("Item name is required.");
            return;
        }

        // Ensure Resources folder exists
        string resourcesPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
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

        // Save the new InventoryItem ScriptableObject to the Resources folder
        string itemPath = resourcesPath + "/" + itemName + ".asset";
        AssetDatabase.CreateAsset(newItem, itemPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Item created successfully.");
    }
}
