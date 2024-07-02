using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickAccessBar : MonoBehaviour
{
    public int totalSlots = 10;
    public GameObject slotPrefab;
    public Transform quickAccessGrid;
    public GameObject buttonNumberTextPrefab; // Assign a Text prefab with the necessary settings
    public List<InventoryItem> allItemsList; // Add this to your script to hold all possible items

    public List<InventorySlot> quickAccessSlots = new List<InventorySlot>();
    private PlayerStatus playerStatus;
    public ItemDB itemDB; // Reference to ItemDB

    private void Start()
    {
        SetupQuickAccessBar();
        playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus component not found on the Player GameObject.");
        }

        if (itemDB != null)
        {
            PopulateAllItemsList();
        }

    }

    public void PopulateAllItemsList()
    {
        allItemsList = itemDB.items;
    }

    private void SetupQuickAccessBar()
    {
        foreach (Transform child in quickAccessGrid)
        {
            Destroy(child.gameObject);
        }
        quickAccessSlots.Clear();

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, quickAccessGrid);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slot.SetTransformProperties();
                quickAccessSlots.Add(slotUI.slot);

                InventoryDragHandler dragHandler = slotObject.transform.Find("DraggableItem").GetComponent<InventoryDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.slot = slotUI.slot;
                }

                // Instantiate and set up button number text
                GameObject buttonNumberTextObject = Instantiate(buttonNumberTextPrefab, slotObject.transform);
                Text buttonNumberText = buttonNumberTextObject.GetComponent<Text>();
                if (buttonNumberText != null)
                {
                    buttonNumberText.text = (i < 9) ? (i + 1).ToString() : "0";
                    RectTransform rectTransform = buttonNumberText.GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.anchoredPosition = new Vector2(0, 8); // Position below the slot
                }
            }
            else
            {
                Debug.LogError($"InventorySlot component not found on slot prefab at index {i}");
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < totalSlots; i++)
        {
            if ((i < 9 && Input.GetKeyDown(KeyCode.Alpha1 + i)) || (i == 9 && Input.GetKeyDown(KeyCode.Alpha0)))
            {
                UseItemInSlot(i);
            }
        }
    }

    public void UseItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= quickAccessSlots.Count)
        {
            Debug.LogError("Invalid slot index");
            return;
        }

        InventorySlot slot = quickAccessSlots[slotIndex];
        if (slot.item != null)
        {
            slot.UseItem();
        }
        else
        {
            Debug.Log($"No item in slot {slotIndex} to use.");
        }
    }

    public void AddItemToQuickAccessBar(InventoryItem item, int quantity = 1)
    {
        foreach (var slot in quickAccessSlots)
        {
            if (slot.item == null)
            {
                slot.SetItem(item, quantity);
                return;
            }
        }
        Debug.LogWarning("Quick access bar is full!");
    }

    public List<InventoryItemData> GetItems()
    {
        List<InventoryItemData> items = new List<InventoryItemData>();
        foreach (var slot in quickAccessSlots)
        {
            if (slot.item != null)
            {
                items.Add(new InventoryItemData { itemName = slot.item.itemName, quantity = slot.quantity });
                Debug.Log($"Saved Quick Access Item: {slot.item.itemName} with Quantity: {slot.quantity}");
            }
        }
        return items;
    }

    public void LoadItems(List<InventoryItemData> items)
    {
        foreach (var itemData in items)
        {
            InventoryItem item = FindItemByName(itemData.itemName);
            if (item != null)
            {
                AddItemToQuickAccessBar(item, itemData.quantity);
                Debug.Log($"Loaded Quick Access Item: {item.itemName} with Quantity: {itemData.quantity}");
            }
            else
            {
                Debug.LogWarning($"Item {itemData.itemName} not found in allItemsList.");
            }
        }
    }

    public void ClearItems()
    {
        foreach (var slot in quickAccessSlots)
        {
            slot.SetItem(null, 0);
        }
    }

    private InventoryItem FindItemByName(string itemName)
    {
        return allItemsList.Find(item => item.itemName == itemName);
    }
}
