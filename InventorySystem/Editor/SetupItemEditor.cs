using UnityEditor;
using UnityEngine;

public class SetupItemEditor : EditorWindow
{
    private InventoryItem selectedItem;
    private GameObject itemPrefab;
    private GameObject pickupTextPrefab;
    private float colliderRadius = 1f;
    private float colliderHeight = -0.5f;

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

        GUILayout.Label("Trigger Collider Settings", EditorStyles.boldLabel);
        colliderRadius = EditorGUILayout.FloatField("Radius", colliderRadius);
        colliderHeight = EditorGUILayout.FloatField("Height Offset", colliderHeight);

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
        itemPickup.pickupTextPrefab = pickupTextPrefab;

        // Create and configure the trigger collider
        SphereCollider triggerCollider = newItemObject.GetComponent<SphereCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = newItemObject.AddComponent<SphereCollider>();
        }
        triggerCollider.isTrigger = true;
        triggerCollider.radius = colliderRadius;
        triggerCollider.center = new Vector3(0, colliderHeight, 0);

        // Save the new item prefab in the Resources folder
        string prefabPath = resourcesPath + "/" + selectedItem.itemName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newItemObject, prefabPath);

        // Destroy the temporary GameObject
        DestroyImmediate(newItemObject);

        Debug.Log("Item setup completed successfully.");
    }
}
