using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickAccessBar : MonoBehaviour
{
    public int totalSlots = 10;
    public GameObject slotPrefab;
    public Transform quickAccessGrid;
    public GameObject buttonNumberTextPrefab; // Assign a Text prefab with the necessary settings

    public List<InventorySlot> quickAccessSlots = new List<InventorySlot>();
    private PlayerStatus playerStatus;

    private void Start()
    {
        SetupQuickAccessBar();
        playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus component not found on the Player GameObject.");
        }
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
                    rectTransform.anchoredPosition = new Vector2(0, -20); // Position below the slot
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
}
