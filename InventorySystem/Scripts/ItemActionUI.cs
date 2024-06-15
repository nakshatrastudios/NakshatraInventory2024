using UnityEngine;
using UnityEngine.UI;

public class ItemActionUI : MonoBehaviour
{
    public Button useButton;
    public Button equipButton;
    public Button dropButton;

    private InventorySlotUI parentSlotUI;

    private Inventory playerInventory;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (playerInventory == null)
            {
                Debug.LogError("Inventory component not found on player.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }
    }

    public void ConfigureButtons(InventoryItem item, InventorySlotUI slotUI)
    {
        parentSlotUI = slotUI;

        useButton.gameObject.SetActive(item.itemType == ItemType.Consumable);
        equipButton.gameObject.SetActive(item.itemType == ItemType.Equipment);
        dropButton.gameObject.SetActive(true); // Drop button is always enabled

        useButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(() => UseItem(item, slotUI.slot));
        equipButton.onClick.AddListener(() => EquipItem(item, slotUI.slot));
        dropButton.onClick.AddListener(() => DropItem(item, slotUI.slot));
    }

    private void UseItem(InventoryItem item, InventorySlot slot)
    {
        Debug.Log($"Using {item.itemName}");
        ApplyItemEffects(item);
        if (playerInventory != null)
        {
            playerInventory.RemoveItemFromSlot(slot, 1); // Remove the used item from the specific slot
        }
        CloseActionUI();
    }

    private void EquipItem(InventoryItem item, InventorySlot slot)
    {
        Debug.Log($"Equipping {item.itemName}");
        // Implement item equipping logic here
        CloseActionUI();
    }

    private void DropItem(InventoryItem item, InventorySlot slot)
    {
        Debug.Log($"Dropping {item.itemName}");
        // Implement item dropping logic here
        if (playerInventory != null)
        {
            playerInventory.RemoveItemFromSlot(slot, 1); // Remove the dropped item from the specific slot
        }
        CloseActionUI();
    }

    private void ApplyItemEffects(InventoryItem item)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                foreach (var stat in item.stats)
                {
                    switch (stat.statType)
                    {
                        case StatType.Health:
                            playerStatus.Health = Mathf.Min(playerStatus.Health + stat.value, playerStatus.MaxHealth);
                            break;
                        case StatType.Mana:
                            playerStatus.Mana = Mathf.Min(playerStatus.Mana + stat.value, playerStatus.MaxMana);
                            break;
                        case StatType.Stamina:
                            playerStatus.Stamina = Mathf.Min(playerStatus.Stamina + stat.value, playerStatus.MaxStamina);
                            break;
                        default:
                            playerStatus.AddStat(stat.statType, stat.value);
                            break;
                    }
                }
                Debug.Log("Item effects applied.");
            }
            else
            {
                Debug.LogError("PlayerStatus component not found on player.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }
    }

    public void CloseActionUI()
    {
        if (parentSlotUI != null)
        {
            parentSlotUI.CloseItemActionUI();
        }
    }
}
