using UnityEngine;
using UnityEngine.UI;

public class ItemActionUI : MonoBehaviour
{
    public Button useButton;
    public Button equipButton;
    public Button dropButton;

    private InventorySlotUI parentSlotUI;

    public void ConfigureButtons(InventoryItem item, InventorySlotUI slotUI)
    {
        parentSlotUI = slotUI;

        useButton.gameObject.SetActive(item.itemType == ItemType.Consumable);
        equipButton.gameObject.SetActive(item.itemType == ItemType.Equipment);
        dropButton.gameObject.SetActive(true); // Drop button is always enabled

        // Optionally, set up button click listeners here
        useButton.onClick.RemoveAllListeners();
        equipButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(() => UseItem(item));
        equipButton.onClick.AddListener(() => EquipItem(item));
        dropButton.onClick.AddListener(() => DropItem(item));
    }

    private void UseItem(InventoryItem item)
    {
        Debug.Log($"Using {item.itemName}");
        // Implement item usage logic here

        CloseActionUI();
    }

    private void EquipItem(InventoryItem item)
    {
        Debug.Log($"Equipping {item.itemName}");
        // Implement item equipping logic here

        CloseActionUI();
    }

    private void DropItem(InventoryItem item)
    {
        Debug.Log($"Dropping {item.itemName}");
        // Implement item dropping logic here

        CloseActionUI();
    }

    public void CloseActionUI()
    {
        if (parentSlotUI != null)
        {
            parentSlotUI.CloseItemActionUI();
        }
    }
}
