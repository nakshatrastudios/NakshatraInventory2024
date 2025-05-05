using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nakshatra.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
    {
        public InventorySlot slot;
        public GameObject itemActionPrefab; // Assign this in the inspector
        private static GameObject currentItemActionUI;
        private Canvas parentCanvas;
        public bool isEquipmentSlot; // Indicates if this slot is for equipment

        private float lastClickTime;
        private const float doubleClickThreshold = 0.3f; // Adjust as needed

        private void Awake()
        {
            slot.slotObject = gameObject;
            slot.stackText = transform.Find("DraggableItem/StackText")?.GetComponent<Text>();
            slot.itemIcon = transform.Find("DraggableItem/ItemIcon")?.GetComponent<Image>();

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
            Debug.Log($"Slot clicked: {gameObject.name}, Button: {eventData.button}, Item: {(slot.item != null ? slot.item.itemName : "None")}");
            if (currentItemActionUI != null)
            {
                Destroy(currentItemActionUI);
                currentItemActionUI = null;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Time.time - lastClickTime < doubleClickThreshold)
                {
                    HandleDoubleClick();
                }
                lastClickTime = Time.time;
            }
            else if (eventData.button == PointerEventData.InputButton.Right && slot.item != null)
            {
                Debug.Log($"Right-clicked on item: {slot.item.itemName}");
                ShowItemActionUI(slot.item);
            }
            else
            {
                Debug.Log("Right-clicked on empty slot or left-clicked.");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var panel = FindObjectOfType<ItemDescriptionPanel>();
            if (panel != null)
                panel.Show(slot.item);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var panel = FindObjectOfType<ItemDescriptionPanel>();
            if (panel != null)
                panel.Hide();
        }

        private void HandleDoubleClick()
        {
            if (slot != null && slot.item != null)
            {
                slot.UseItem();
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
                Equipment playerEquipment = GameObject.FindWithTag("Player")?.GetComponent<Equipment>();
                itemActionUI.ConfigureButtons(item, this, playerEquipment);
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

        private void OnEnable()
        {
            // Ensure the UI gets updated when the canvas is enabled
            slot.SetItem(slot.item, slot.quantity);
        }
    }
}