using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public InventorySlot helmetSlot;
    public InventorySlot shoulderSlot;
    public InventorySlot torsoSlot;
    public InventorySlot pantsSlot;
    public InventorySlot glovesSlot;
    public InventorySlot bootsSlot;

    public Dictionary<EquipmentCategory, InventorySlot> equipmentSlots;

    private Inventory inventory;

    private void Start()
    {
        equipmentSlots = new Dictionary<EquipmentCategory, InventorySlot>
        {
            { EquipmentCategory.Helmet, helmetSlot },
            { EquipmentCategory.Shoulder, shoulderSlot },
            { EquipmentCategory.Torso, torsoSlot },
            { EquipmentCategory.Pants, pantsSlot },
            { EquipmentCategory.Gloves, glovesSlot },
            { EquipmentCategory.Boots, bootsSlot }
        };

        foreach (var slot in equipmentSlots.Values)
        {
            InventorySlotUI slotUI = slot.slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.isEquipmentSlot = true;
                Debug.Log($"Equipment slot set up: {slot.slotObject.name}");
            }
            else
            {
                Debug.LogError("InventorySlotUI component missing on equipment slot.");
            }
        }

        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found in the scene.");
        }
    }

    public bool IsItemEquipped(InventoryItem item)
    {
        foreach (var slot in equipmentSlots.Values)
        {
            if (slot.item == item)
            {
                return true;
            }
        }
        return false;
    }

    public void EquipItem(InventoryItem item)
    {
        if (equipmentSlots.TryGetValue(item.equipmentCategory, out InventorySlot slot))
        {
            if (slot.item != null)
            {
                UnequipItem(slot.item);
            }

            slot.SetItem(item, 1);
            PlayerStatus playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.AddStats(item.stats);
            }

            InventorySlotUI slotUI = slot.slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slot = slot;
                Debug.Log($"Updated InventorySlotUI for slot: {slot.slotObject.name} with item: {item.itemName}");
            }
            else
            {
                Debug.LogError("InventorySlotUI component not found on slotObject.");
            }

            Debug.Log($"Item equipped: {item.itemName} in slot: {slot.slotObject.name}");
        }
        else
        {
            Debug.LogError($"No slot available for equipment category: {item.equipmentCategory}");
        }
    }

    public void UnequipItem(InventoryItem item)
    {
        if (equipmentSlots.TryGetValue(item.equipmentCategory, out InventorySlot slot) && slot.item == item)
        {
            PlayerStatus playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.RemoveStats(item.stats);
            }

            slot.SetItem(null, 0);

            // Add item back to inventory
            if (inventory != null)
            {
                inventory.AddItem(item, 1);
            }

            InventorySlotUI slotUI = slot.slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slot = slot;
                Debug.Log($"Updated InventorySlotUI for slot: {slot.slotObject.name} after unequipping item: {item.itemName}");
            }
            else
            {
                Debug.LogError("InventorySlotUI component not found on slotObject.");
            }

            Debug.Log($"Item unequipped: {item.itemName} from slot: {slot.slotObject.name}");

            // Reset the CanvasGroup properties when unequipping the item
            CanvasGroup canvasGroup = slot.slotObject.GetComponentInChildren<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Debug.LogError($"Item {item.itemName} is not equipped in the expected slot or no slot found.");
        }
    }
}
