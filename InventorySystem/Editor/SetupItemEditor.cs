using UnityEditor;
using UnityEngine;

public class SetupItemEditor : EditorWindow
{
    private InventoryItem selectedItem;
    private GameObject itemPrefab;
    private GameObject pickupTextPrefab;
    private Vector3 colliderSize = new Vector3(5, 1, 5);

    [MenuItem("Inventory System/Setup Item")]
    public static void ShowWindow()
    {
        GetWindow<SetupItemEditor>("Setup Item");
    }

    private void OnGUI()
    {
        GUILayout.Label("Setup Item", EditorStyles.boldLabel);
        selectedItem = (InventoryItem)EditorGUILayout.ObjectField("Item", selectedItem, typeof(InventoryItem), false);
        itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", itemPrefab, typeof(GameObject), false);
        pickupTextPrefab = (GameObject)EditorGUILayout.ObjectField("Pickup Text Prefab", pickupTextPrefab, typeof(GameObject), false);

        GUILayout.Label("Trigger Collider Size", EditorStyles.boldLabel);
        colliderSize = EditorGUILayout.Vector3Field("Size", colliderSize);

        if (GUILayout.Button("Setup Item"))
        {
            SetupItem();
        }
    }

    private void SetupItem()
    {
        if (selectedItem == null)
        {
            Debug.LogError("Please select an item.");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab is required.");
            return;
        }

        // Ensure Resources folder exists
        string resourcesPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Create a new GameObject for the item
        GameObject newItemObject = Instantiate(itemPrefab);
        newItemObject.name = selectedItem.itemName;
        ItemPickup itemPickup = newItemObject.GetComponent<ItemPickup>();
        if (itemPickup == null)
        {
            itemPickup = newItemObject.AddComponent<ItemPickup>();
        }
        itemPickup.item = selectedItem;

        // Create and configure the trigger collider
        BoxCollider triggerCollider = newItemObject.GetComponent<BoxCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = newItemObject.AddComponent<BoxCollider>();
        }
        triggerCollider.isTrigger = true;
        triggerCollider.size = colliderSize;

        // Add the PickupText prefab to the item
        if (pickupTextPrefab != null)
        {
            GameObject pickupTextObject = Instantiate(pickupTextPrefab, newItemObject.transform);
            pickupTextObject.name = "PickupText";
            pickupTextObject.SetActive(false); // Hide initially
            itemPickup.pickupText = pickupTextObject;
        }

        // Save the new item prefab in the Resources folder
        string prefabPath = resourcesPath + "/" + selectedItem.itemName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newItemObject, prefabPath);

        // Destroy the temporary GameObject
        DestroyImmediate(newItemObject);

        Debug.Log("Item setup completed successfully.");
    }
}
