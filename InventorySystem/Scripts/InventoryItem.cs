using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName = "New Item";
    public string itemDescription = "Item Description";
    public Sprite itemIcon = null;
    public bool isStackable = false;
    public int maxStackSize = 1;

    public ItemType itemType;

    // Equipment specific
    public EquipmentCategory equipmentCategory;

    public int amount;

    // Weapon specific
    public WeaponType weaponType;
    public bool isMainHand;
    public bool isOffHand;

    // Dynamic stats
    public List<ItemStat> stats = new List<ItemStat>();

    public bool setBone = false;

    // Currency specific
    public Dictionary<string, int> currencyAmounts = new Dictionary<string, int>();

    // New fields for bone attachment
    public List<string> selectedBoneNames = new List<string>();
    public List<GameObject> itemPrefabs = new List<GameObject>();
    public List<Vector3> itemPositions = new List<Vector3>();
    public List<Vector3> itemRotations = new List<Vector3>();

    // Pickup specific
    public GameObject itemPickupPrefab;
    public GameObject pickupTextPrefab;
}

public enum ItemType
{
    Consumable,
    Equipment,
    Currency,
    Other
}

public enum EquipmentCategory
{
    Weapon,
    Helmet,
    Torso,
    Gloves,
    Shoulder,
    Boots,
    Pants,
    Rings,
    Necklace,
    Belt,
    Earrings,
    Back,
}

public enum WeaponType
{
    OneHand,
    TwoHand
}

[System.Serializable]
public class ItemStat
{
    public StatType statType;
    public int value;
}

public enum StatType
{
    Attack,
    Defense,
    Block,
    Intelligence,
    Health,
    MaxHealth,
    Mana,
    MaxMana,
    Stamina,
    MaxStamina,
    Speed,
    Agility,
    Strength,
    Dexterity,
    Luck
}
