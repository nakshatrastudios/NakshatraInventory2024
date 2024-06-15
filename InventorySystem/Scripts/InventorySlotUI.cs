using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot slot;
    public GameObject itemActionPrefab; // Assign this in the inspector
    private static GameObject currentItemActionUI;
    private Canvas parentCanvas;
    //public Inventory inventory;

    private void Awake()
    {
        slot = new InventorySlot
        {
            slotObject = gameObject,
            stackText = transform.Find("DraggableItem/StackText")?.GetComponent<Text>(),
            itemIcon = transform.Find("DraggableItem/ItemIcon")?.GetComponent<Image>()
        };

        if (slot.stackText == null)
        {
            Debug.LogError($"StackText not found in DraggableItem for slot: {gameObject.name}");
        }

        if (slot.itemIcon == null)
        {
            Debug.LogError($"ItemIcon not found in DraggableItem for slot: {gameObject.name}");
        }

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("Parent canvas not found.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItemActionUI != null)
        {
            Destroy(currentItemActionUI);
            currentItemActionUI = null;
        }

        if (eventData.button == PointerEventData.InputButton.Right && slot.item != null)
        {
            ShowItemActionUI(slot.item);
        }
    }

    private void Update()
    {
        if (currentItemActionUI != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            Vector2 localMousePosition = parentCanvas.transform.InverseTransformPoint(Input.mousePosition);
            RectTransform rectTransform = currentItemActionUI.GetComponent<RectTransform>();

            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, parentCanvas.worldCamera))
            {
                Destroy(currentItemActionUI);
                currentItemActionUI = null;
            }
        }
    }

    private void ShowItemActionUI(InventoryItem item)
    {
        if (currentItemActionUI != null)
        {
            Destroy(currentItemActionUI);
        }

        currentItemActionUI = Instantiate(itemActionPrefab, parentCanvas.transform);

        // Convert screen position to canvas position
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out anchoredPosition);

        currentItemActionUI.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        // Ensure the size of the ItemAction prefab is correct
        RectTransform rectTransform = currentItemActionUI.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(100, 100);
        }

        ItemActionUI itemActionUI = currentItemActionUI.GetComponent<ItemActionUI>();
        if (itemActionUI != null)
        {
            itemActionUI.ConfigureButtons(item, this);
        }
    }

    public void CloseItemActionUI()
    {
        if (currentItemActionUI != null)
        {
            Destroy(currentItemActionUI);
            currentItemActionUI = null;
        }
    }
}
