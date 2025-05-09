using System.Collections.Generic;
using UnityEngine;
using Nakshatra.InventorySystem;

namespace Nakshatra.InventorySystem
{
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
        public List<Vector3> itemScale = new List<Vector3>();

        // Pickup specific
        public GameObject itemPickupPrefab;
        public GameObject pickupTextPrefab;

        [Header("Sounds (Optional)")]
        [Tooltip("Played when this item is picked up by the player")]
        public AudioClip pickupSound;

        [Header("Sounds (Optional)")]
        [Tooltip("Played when this item is equipped (or used if consumable)")]
        public AudioClip onEquipSound;

        [Tooltip("Played when this item is unequipped")]
        public AudioClip onUnequipSound;

        [Header("Sibling Toggles (Optional)")]
        public bool toggleSiblings;
        public List<ParentToggleData> parentToggles = new List<ParentToggleData>();

    }

    [System.Serializable]
    public class ParentToggleData
    {
        [Tooltip("Name of the parent GameObject in the scene")]
        public string parentName;

        [Tooltip("Child names to ENABLE on Equip (all others will be disabled)")]
        public List<string> enableOnEquip = new List<string>();

        [Tooltip("Child names to ENABLE on Unequip (all others will be disabled)")]
        public List<string> enableOnUnequip = new List<string>();
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
}