using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int rows = 4;
    public int columns = 5;
    public int totalSlots = 20;
    public GameObject slotPrefab;
    public Transform inventoryGrid;
    public Button nextPageButton;
    public Button previousPageButton;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int currentPage = 0;
    private int pages;

    public int Pages
    {
        get { return pages; }
    }

    void Start()
    {
        SetupInventoryUI();
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
    }

    public void SetupInventoryUI()
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        int slotsPerPage = rows * columns;
        pages = Mathf.CeilToInt((float)totalSlots / slotsPerPage);

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, inventoryGrid);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slot.SetTransformProperties();
                inventorySlots.Add(slotUI.slot);

                InventoryDragHandler dragHandler = slotObject.transform.Find("DraggableItem").GetComponent<InventoryDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.slot = slotUI.slot;
                }
            }
        }

        UpdatePage();
    }

    public void AddItem(InventoryItem item, int quantity = 1)
    {
        while (quantity > 0)
        {
            InventorySlot existingSlot = FindItemSlot(item);

            if (existingSlot != null)
            {
                int amountToAdd = Mathf.Min(quantity, item.maxStackSize - existingSlot.quantity);
                existingSlot.quantity += amountToAdd;
                existingSlot.stackText.text = existingSlot.quantity > 1 ? existingSlot.quantity.ToString() : "";
                existingSlot.stackText.enabled = existingSlot.quantity > 1;
                quantity -= amountToAdd;
            }
            else
            {
                InventorySlot newSlot = inventorySlots.Find(slot => slot.item == null);
                if (newSlot != null)
                {
                    int amountToAdd = Mathf.Min(quantity, item.maxStackSize);
                    newSlot.SetItem(item, amountToAdd);
                    newSlot.SetTransformProperties();
                    quantity -= amountToAdd;
                }
                else
                {
                    Debug.LogWarning("Inventory is full!");
                    break;
                }
            }
        }
    }

    public void RemoveItem(InventoryItem item, int quantity = 1)
    {
        InventorySlot existingSlot = FindItemSlot(item);

        if (existingSlot != null)
        {
            existingSlot.quantity -= quantity;
            if (existingSlot.quantity <= 0)
            {
                existingSlot.SetItem(null, 0);
            }
            else
            {
                existingSlot.stackText.text = existingSlot.quantity > 1 ? existingSlot.quantity.ToString() : "";
                existingSlot.stackText.enabled = existingSlot.quantity > 1;
            }
        }
        else
        {
            InventorySlot slot = inventorySlots.Find(s => s.item == item);
            if (slot != null)
            {
                slot.SetItem(null, 0);
            }
            else
            {
                Debug.LogWarning("Item not found in inventory!");
            }
        }
    }

    public void RemoveItemFromSlot(InventorySlot slot, int quantity)
    {
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0)
            {
                slot.SetItem(null, 0);
            }
            else
            {
                if (slot.quantity > 1)
                {
                    slot.stackText.text = slot.quantity.ToString();
                    slot.stackText.enabled = true;
                }
                else
                {
                    slot.stackText.text = "";
                    slot.stackText.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("Slot not found in inventory!");
        }
    }

    private InventorySlot FindItemSlot(InventoryItem item)
    {
        return inventorySlots.Find(slot => slot.item == item && slot.quantity < item.maxStackSize);
    }

    public void SetPage(int pageIndex)
    {
        currentPage = pageIndex;
        UpdatePage();
    }

    public void NextPage()
    {
        if (currentPage < Pages - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    public void UpdatePage()
    {
        int slotsPerPage = rows * columns;
        int startSlot = currentPage * slotsPerPage;
        int endSlot = Mathf.Min(startSlot + slotsPerPage, totalSlots);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i >= startSlot && i < endSlot)
            {
                inventorySlots[i].slotObject.SetActive(true);
            }
            else
            {
                inventorySlots[i].slotObject.SetActive(false);
            }
        }

        nextPageButton.interactable = currentPage < Pages - 1;
        previousPageButton.interactable = currentPage > 0;
    }
}
