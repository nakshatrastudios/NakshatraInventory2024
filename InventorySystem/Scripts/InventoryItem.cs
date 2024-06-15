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

    // Weapon specific
    public WeaponType weaponType;
    public bool isMainHand;
    public bool isOffHand;

    // Dynamic stats
    public List<ItemStat> stats = new List<ItemStat>();
}

public enum ItemType
{
    Consumable,
    Equipment,
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
    Back // For cloak
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
