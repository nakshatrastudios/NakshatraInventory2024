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
    private Canvas dragCanvas;

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

            // Create a new canvas for dragging
            dragCanvas = new GameObject("DragCanvas").AddComponent<Canvas>();
            dragCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            dragCanvas.sortingOrder = 1000; // Ensure this is on top
            dragItem = new GameObject("DragItem");
            dragItem.transform.SetParent(dragCanvas.transform, false);

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
        if (dragRectTransform != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragCanvas.transform as RectTransform, Input.mousePosition, eventData.pressEventCamera, out position);
            dragRectTransform.localPosition = position;

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

        if (dragCanvas != null)
        {
            Destroy(dragCanvas.gameObject);
            dragCanvas = null;
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

                if (draggedSlot.item == null)
                {
                    Debug.LogError("Dragged item is null.");
                    return;
                }

                if (targetSlot == null)
                {
                    Debug.LogError("Target slot is null.");
                    return;
                }

                // Check if target slot is an equipment slot
                if (targetSlot.slotObject.GetComponent<InventorySlotUI>().isEquipmentSlot)
                {
                    Equipment equipment = GameObject.FindWithTag("Player").GetComponent<Equipment>();
                    if (equipment != null)
                    {
                        InventorySlot correctSlot = equipment.GetTargetSlot(draggedSlot.item);
                        if (draggedSlot.item.itemType == ItemType.Equipment &&
                            correctSlot != null &&
                            correctSlot.slotObject == targetSlot.slotObject)
                        {
                            equipment.EquipItem(draggedSlot.item);
                            draggedSlot.SetItem(null, 0);
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
                        Debug.LogError("Equipment component not found on the Player GameObject.");
                    }
                }
                else
                {
                    // Handle unequipping logic if dragging from equipment slot to inventory slot
                    if (draggedSlot.slotObject.GetComponent<InventorySlotUI>().isEquipmentSlot && !targetSlot.slotObject.GetComponent<InventorySlotUI>().isEquipmentSlot)
                    {
                        Equipment equipment = GameObject.FindWithTag("Player").GetComponent<Equipment>();
                        if (equipment != null)
                        {
                            equipment.UnequipItem(draggedSlot.item);
                        }
                    }

                    // Swap items between slots
                    InventoryItem tempItem = targetSlot.item;
                    int tempQuantity = targetSlot.quantity;

                    if (draggedSlot.slotObject.GetComponent<InventorySlotUI>().isEquipmentSlot)
                    {
                        targetSlot.SetItem(tempItem, tempQuantity);
                        draggedSlot.SetItem(null, 0);
                    }
                    else
                    {
                        targetSlot.SetItem(draggedSlot.item, draggedSlot.quantity);
                        draggedSlot.SetItem(tempItem, tempQuantity);
                    }
                }

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

    private void Update()
    {
        if (dragItem != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragCanvas.transform as RectTransform, Input.mousePosition, null, out position);
            dragRectTransform.localPosition = position;
        }
    }
}
