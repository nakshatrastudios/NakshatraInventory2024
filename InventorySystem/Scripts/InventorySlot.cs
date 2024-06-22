using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;
    public int quantity;
    public GameObject slotObject;
    public Text stackText;
    public Image itemIcon;

    private CanvasGroup canvasGroup;

    public void SetItem(InventoryItem newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;

        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 1); // Make the icon fully visible

            stackText.text = quantity > 1 ? quantity.ToString() : "";
            stackText.enabled = true;
            Debug.Log($"Item set: {item.itemName} in slot: {slotObject.name} with quantity: {quantity}");
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = true;
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0);
            stackText.text = "";
            stackText.enabled = true;
            Debug.Log($"Item removed from slot: {slotObject.name}");
        }

        SetTransformProperties();

        // Reset the CanvasGroup properties when the item is set or removed
        if (canvasGroup == null)
        {
            canvasGroup = slotObject.GetComponentInChildren<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void SetTransformProperties()
    {
        if (slotObject == null || itemIcon == null || stackText == null)
        {
            Debug.LogError("One or more required components are not assigned in InventorySlot.");
            return;
        }

        // Find DraggableItem RectTransform
        RectTransform draggableItemRect = slotObject.transform.Find("DraggableItem")?.GetComponent<RectTransform>();
        if (draggableItemRect == null)
        {
            Debug.LogError($"DraggableItem RectTransform not found in {slotObject.name}");
            return;
        }

        // Set DraggableItem RectTransform properties
        draggableItemRect.anchorMin = new Vector2(0.5f, 0.5f);
        draggableItemRect.anchorMax = new Vector2(0.5f, 0.5f);
        draggableItemRect.offsetMin = Vector2.zero;
        draggableItemRect.offsetMax = Vector2.zero;
        draggableItemRect.sizeDelta = new Vector2(56, 56);

        // Set ItemIcon RectTransform properties
        if (itemIcon != null)
        {
            RectTransform itemIconRect = itemIcon.GetComponent<RectTransform>();
            itemIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemIconRect.pivot = new Vector2(0.5f, 0.5f);
            itemIconRect.sizeDelta = new Vector2(80, 80); // 80% of the slot size
            itemIconRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogError("ItemIcon RectTransform not found.");
        }

        // Set StackText RectTransform properties
        if (stackText != null)
        {
            RectTransform stackTextRect = stackText.GetComponent<RectTransform>();
            stackTextRect.anchorMin = new Vector2(0, 0.5f);
            stackTextRect.anchorMax = new Vector2(0, 0.5f);
            stackTextRect.pivot = new Vector2(0, 1);
            stackTextRect.sizeDelta = new Vector2(56, 28);
            stackTextRect.anchoredPosition = new Vector2(0, 0); // Adjust as needed
        }
        else
        {
            Debug.LogError("StackText RectTransform not found.");
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            Debug.Log($"Used item: {item.itemName}");
            if (item.itemType == ItemType.Consumable)
            {
                PlayerStatus playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    playerStatus.AddStats(item.stats);
                }

                quantity--;
                if (quantity <= 0)
                {
                    SetItem(null, 0);
                }
                else
                {
                    stackText.text = quantity.ToString();
                }
            }
            else if (item.itemType == ItemType.Equipment)
            {
                Equipment equipment = GameObject.FindWithTag("Player").GetComponent<Equipment>();
                if (equipment != null)
                {
                    equipment.EquipItem(item);
                    quantity--;
                    if (quantity <= 0)
                    {
                        SetItem(null, 0);
                    }
                    else
                    {
                        stackText.text = "";
                    }
                }
                else
                {
                    Debug.LogError("Equipment component not found on the Player GameObject.");
                }
            }
        }
        else
        {
            Debug.Log("No item to use.");
        }
    }
}
