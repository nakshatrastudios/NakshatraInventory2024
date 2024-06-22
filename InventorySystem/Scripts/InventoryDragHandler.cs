using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public InventorySlot slot;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Inventory inventory;
    private Equipment equipment;
    private bool isChangingPage = false;

    private GameObject dragItem;
    private RectTransform dragRectTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        inventory = FindObjectOfType<Inventory>();
        equipment = FindObjectOfType<Equipment>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            dragItem = new GameObject("DragItem");
            dragItem.transform.SetParent(canvas.transform, true);
            dragItem.transform.SetAsLastSibling();

            Image dragImage = dragItem.AddComponent<Image>();
            dragImage.sprite = slot.itemIcon.sprite;
            dragImage.SetNativeSize();

            CanvasGroup tempCanvasGroup = dragItem.AddComponent<CanvasGroup>();
            tempCanvasGroup.blocksRaycasts = false;

            dragRectTransform = dragItem.GetComponent<RectTransform>();
            dragRectTransform.sizeDelta = rectTransform.sizeDelta;
            dragRectTransform.position = Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragItem != null)
        {
            dragRectTransform.position = Input.mousePosition;

            if (inventory != null && inventory.nextPageButton != null && inventory.previousPageButton != null)
            {
                RectTransform nextButtonRect = inventory.nextPageButton.GetComponent<RectTransform>();
                RectTransform prevButtonRect = inventory.previousPageButton.GetComponent<RectTransform>();

                if (!isChangingPage && RectTransformUtility.RectangleContainsScreenPoint(nextButtonRect, Input.mousePosition, canvas.worldCamera))
                {
                    isChangingPage = true;
                    inventory.NextPage();
                    Invoke(nameof(ResetPageChange), 0.5f);
                }
                else if (!isChangingPage && RectTransformUtility.RectangleContainsScreenPoint(prevButtonRect, Input.mousePosition, canvas.worldCamera))
                {
                    isChangingPage = true;
                    inventory.PreviousPage();
                    Invoke(nameof(ResetPageChange), 0.5f);
                }
            }
        }
    }

    private void ResetPageChange()
    {
        isChangingPage = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragItem != null)
        {
            Destroy(dragItem);
            dragItem = null;
        }

        if (slot.item != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            InventorySlot targetSlot = null;
            if (eventData.pointerEnter != null)
            {
                var targetSlotObject = eventData.pointerEnter.GetComponentInParent<InventorySlotUI>();
                if (targetSlotObject != null)
                {
                    targetSlot = targetSlotObject.slot;
                }
            }

            if (targetSlot == null || targetSlot == slot)
            {
                rectTransform.anchoredPosition = originalPosition;
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }

            if (targetSlot != null)
            {
                var targetHandler = targetSlot.slotObject.transform.Find("DraggableItem")?.GetComponent<CanvasGroup>();
                if (targetHandler != null)
                {
                    targetHandler.alpha = 1f;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryDragHandler draggedHandler = eventData.pointerDrag.GetComponent<InventoryDragHandler>();
            if (draggedHandler != null && draggedHandler.slot != null)
            {
                InventorySlot draggedSlot = draggedHandler.slot;
                InventorySlot targetSlot = slot;

                Debug.Log($"Dragging item: {draggedSlot.item?.itemName} to target slot: {targetSlot.slotObject.name}");

                // Check if the target slot is valid for the dragged item
                if (targetSlot != null && draggedSlot != null)
                {
                    // If target slot is an equipment slot
                    if (targetSlot.slotObject.GetComponent<InventorySlotUI>().isEquipmentSlot)
                    {
                        Debug.Log($"Target slot {targetSlot.slotObject.name} is an equipment slot.");
                        if (draggedSlot.item != null)
                        {
                            Debug.Log($"Dragged item category: {draggedSlot.item.equipmentCategory}");
                        }
                        else
                        {
                            Debug.LogError("Dragged item is null.");
                        }

                        if (equipment != null && equipment.equipmentSlots.TryGetValue(draggedSlot.item.equipmentCategory, out InventorySlot correctSlot))
                        {
                            if (correctSlot == targetSlot)
                            {
                                equipment.EquipItem(draggedSlot.item);
                                draggedSlot.SetItem(null, 0);
                                Debug.Log($"Equipped item {draggedSlot.item?.itemName} to {targetSlot.slotObject.name}");
                            }
                            else
                            {
                                Debug.LogError("Invalid equipment slot!");
                                draggedHandler.rectTransform.anchoredPosition = draggedHandler.originalPosition; // Snap back to original position
                                return;
                            }
                        }
                        else
                        {
                            Debug.LogError("Equipment slot not found or Equipment component is null.");
                            draggedHandler.rectTransform.anchoredPosition = draggedHandler.originalPosition; // Snap back to original position
                            return;
                        }
                    }
                    else
                    {
                        InventoryItem tempItem = draggedSlot.item;
                        int tempQuantity = draggedSlot.quantity;

                        draggedSlot.SetItem(targetSlot.item, targetSlot.quantity);
                        targetSlot.SetItem(tempItem, tempQuantity);

                        draggedHandler.rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.anchoredPosition = Vector2.zero;

                        draggedSlot.SetTransformProperties();
                        targetSlot.SetTransformProperties();

                        draggedHandler.canvasGroup.blocksRaycasts = true;
                        canvasGroup.blocksRaycasts = true;

                        draggedHandler.canvasGroup.alpha = 1f;
                        canvasGroup.alpha = 1f;

                        // Ensure the draggedHandler's dragItem is destroyed when the drop is completed
                        if (draggedHandler.dragItem != null)
                        {
                            Destroy(draggedHandler.dragItem);
                            draggedHandler.dragItem = null;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (dragItem != null)
        {
            dragRectTransform.position = Input.mousePosition;
        }
    }
}
