using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(InventoryItem))]
public class InventoryItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventoryItem item = (InventoryItem)target;

        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        item.itemDescription = EditorGUILayout.TextField("Item Description", item.itemDescription);
        item.itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", item.itemIcon, typeof(Sprite), false);
        item.isStackable = EditorGUILayout.Toggle("Is Stackable", item.isStackable);
        if (item.isStackable)
        {
            item.maxStackSize = EditorGUILayout.IntField("Max Stack Size", item.maxStackSize);
        }

        item.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);

        if (item.itemType == ItemType.Consumable)
        {
            //item.amount = EditorGUILayout.IntField("Amount", item.amount);
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

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

        if (item.stats == null)
        {
            item.stats = new List<ItemStat>();
        }

        int statToRemove = -1;
        for (int i = 0; i < item.stats.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            item.stats[i].statType = (StatType)EditorGUILayout.EnumPopup(item.stats[i].statType);
            item.stats[i].value = EditorGUILayout.IntField(item.stats[i].value);
            if (GUILayout.Button("Remove"))
            {
                statToRemove = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (statToRemove > -1)
        {
            item.stats.RemoveAt(statToRemove);
        }

        if (GUILayout.Button("Add Stat"))
        {
            item.stats.Add(new ItemStat());
        }

        EditorUtility.SetDirty(item);
    }
}
