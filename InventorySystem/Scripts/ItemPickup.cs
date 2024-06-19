using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;  // Assign this in the Inspector
    public GameObject pickupTextPrefab;  // Assign the PickupText prefab in the Inspector
    private GameObject pickupTextInstance;
    private bool playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        HandleCollisionEnter(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollisionEnter(other.gameObject);
    }

    private void HandleCollisionEnter(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pickupTextPrefab != null && pickupTextInstance == null)
            {
                pickupTextInstance = Instantiate(pickupTextPrefab, transform.position, Quaternion.identity);
                pickupTextInstance.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HandleCollisionExit(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HandleCollisionExit(other.gameObject);
    }

    private void HandleCollisionExit(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pickupTextInstance != null)
            {
                Destroy(pickupTextInstance);
                pickupTextInstance = null;
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Access the player's inventory (assumes player has an Inventory component)
            Inventory playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
            if (playerInventory != null)
            {
                // Add the item to the player's inventory
                playerInventory.AddItem(item, 1);
                // Destroy the pickup text and the item pickup GameObject
                if (pickupTextInstance != null)
                {
                    Destroy(pickupTextInstance);
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player does not have an Inventory component");
            }
        }
    }
}
