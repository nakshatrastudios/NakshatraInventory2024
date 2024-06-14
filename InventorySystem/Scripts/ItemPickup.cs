using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;  // Assign this in the Inspector
    public GameObject pickupText;  // Assign the Text GameObject in the Inspector
    private bool playerInRange;

    private void Start()
    {
        if (pickupText != null)
        {
            pickupText.SetActive(false);  // Hide the text initially
        }
    }

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
            if (pickupText != null)
            {
                pickupText.SetActive(true);  // Show the pickup text
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
            if (pickupText != null)
            {
                pickupText.SetActive(false);  // Hide the pickup text
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
                // Hide the pickup text
                if (pickupText != null)
                {
                    pickupText.SetActive(false);
                }
                // Destroy the item pickup GameObject
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player does not have an Inventory component");
            }
        }
    }
}
