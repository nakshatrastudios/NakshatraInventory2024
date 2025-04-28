using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main Inventory Manager window to create Inventory, Items, Quick Access Bar,
/// Item Database, and Equipment Panel directly from one interface.
/// </summary>
public class InventoryManagerEditor : InventoryManagerBaseEditor
{
    private enum Tab
    {
        CreateInventory,
        CreateItem,
        CreateQuickAccessBar,
        ItemDatabase,
        CreateEquipmentPanel
    }

    private Tab currentTab = Tab.CreateInventory;

    private CreateInventoryEditor      createInventoryEditor;
    private CreateItemEditor           createItemEditor;
    private CreateQuickAccessBarEditor createQuickAccessBarEditor;
    private ItemDatabaseEditor         itemDatabaseEditor;

    // Field for Equipment Panel prefab
    private GameObject equipmentPanelPrefab;
    // Field for the Item Database ScriptableObject
    private ItemDB itemDB;

    [MenuItem("Inventory System/Inventory Manager")]
    public static void ShowWindow()
    {
        GetWindow<InventoryManagerEditor>("Inventory System");
    }

    private void OnEnable()
    {
        createInventoryEditor       = new CreateInventoryEditor();
        createItemEditor            = new CreateItemEditor();
        createQuickAccessBarEditor  = new CreateQuickAccessBarEditor();
        itemDatabaseEditor          = new ItemDatabaseEditor();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawTabs();

        EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 160), GUILayout.ExpandWidth(true));
        BeginScroll();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();

        switch (currentTab)
        {
            case Tab.CreateInventory:
                createInventoryEditor.DrawCreateInventory();
                break;
            case Tab.CreateItem:
                createItemEditor.DrawCreateAndSetupItem();
                break;
            case Tab.CreateQuickAccessBar:
                createQuickAccessBarEditor.DrawCreateQuickAccessBar();
                break;
            case Tab.ItemDatabase:
                itemDatabaseEditor.DrawItemDatabase();
                break;
            case Tab.CreateEquipmentPanel:
                DrawCreateEquipmentPanel();
                break;
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(20);
        EditorGUILayout.EndHorizontal();
        EndScroll();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTabs()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(150));
        GUILayout.Label("Inventory Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Inventory"))        currentTab = Tab.CreateInventory;
        if (GUILayout.Button("Create Item"))             currentTab = Tab.CreateItem;
        if (GUILayout.Button("Create Quick Access Bar")) currentTab = Tab.CreateQuickAccessBar;
        if (GUILayout.Button("Item Database"))           currentTab = Tab.ItemDatabase;
        if (GUILayout.Button("Create Equipment Panel"))  currentTab = Tab.CreateEquipmentPanel;

        EditorGUILayout.EndVertical();
    }

    private void DrawCreateEquipmentPanel()
    {
        GUILayout.Label("Equipment Panel Setup", EditorStyles.boldLabel);
        equipmentPanelPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Equipment Panel Prefab",
            equipmentPanelPrefab,
            typeof(GameObject),
            false
        );
        // Allow user to assign the ItemDB used by Equipment
        itemDB = (ItemDB)EditorGUILayout.ObjectField(
            "Item Database",
            itemDB,
            typeof(ItemDB),
            false
        );

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(equipmentPanelPrefab == null || itemDB == null);
        if (GUILayout.Button("Create Equipment Panel"))
        {
            InstantiateEquipmentPanel();
        }
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// Instantiates the Equipment Panel prefab and populates the Player's Equipment component.
    /// Ensures the Player has an Equipment script, adding it if necessary.
    /// </summary>
    private void InstantiateEquipmentPanel()
    {
        // Instantiate the prefab
        var instance = PrefabUtility.InstantiatePrefab(equipmentPanelPrefab) as GameObject;
        if (instance == null)
        {
            Debug.LogError("Failed to instantiate Equipment Panel prefab.");
            return;
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create Equipment Panel");
        Selection.activeGameObject = instance;
        EditorSceneManager.MarkSceneDirty(instance.scene);

        // Find or add the Player's Equipment component
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Player object with tag 'Player' not found.");
            return;
        }
        var equip = playerObj.GetComponent<Equipment>();
        if (equip == null)
        {
            equip = Undo.AddComponent<Equipment>(playerObj);
        }

        // Assign ItemDB and populate list
        equip.itemDB = itemDB;
        equip.PopulateAllItemsList();

        // Locate the Equipment root under the canvas
        var root = instance.transform.Find("Equipment");
        if (root == null)
        {
            Debug.LogError("Equipment root not found in prefab.");
            return;
        }

        // Assign all slots
        AssignSlot(ref equip.helmetSlot,   root, "HelmetSlot");
        AssignSlot(ref equip.shoulderSlot, root, "ShoulderSlot");
        AssignSlot(ref equip.torsoSlot,    root, "TorsoSlot");
        AssignSlot(ref equip.pantsSlot,    root, "PantSlot");
        AssignSlot(ref equip.glovesSlot,   root, "GlovesSlot");
        AssignSlot(ref equip.bootsSlot,    root, "BootsSlot");
        AssignSlot(ref equip.cloakSlot,    root, "CloakSlot");
        AssignSlot(ref equip.neckSlot,     root, "NeckSlot");
        AssignSlot(ref equip.earRingSlot,  root, "EarRingSlot");
        AssignSlot(ref equip.beltSlot,     root, "BeltSlot");
        AssignSlot(ref equip.ring1Slot,    root, "RingSlot1");
        AssignSlot(ref equip.ring2Slot,    root, "RingSlot2");
        AssignSlot(ref equip.mainHandSlot, root, "MainHandSlot");
        AssignSlot(ref equip.offHandSlot,  root, "OffHandSlot");

        Debug.Log("Equipment Panel instantiated and Equipment script populated on Player.");
    }

    /// <summary>
    /// Helper to assign InventorySlot fields from the EquipmentCanvas hierarchy.
    /// </summary>
    private void AssignSlot(ref InventorySlot slotField, Transform root, string slotName)
    {
        var slotObj = root.Find(slotName)?.gameObject;
        if (slotObj == null)
        {
            Debug.LogWarning($"Slot GameObject '{slotName}' not found.");
            return;
        }
        slotField.slotObject = slotObj;

        var draggable = slotObj.transform.Find("DraggableItem");
        if (draggable != null)
        {
            var icon = draggable.Find("ItemIcon")?.GetComponent<Image>();
            var txt  = draggable.Find("StackText")?.GetComponent<Text>();
            if (icon != null) slotField.itemIcon = icon;
            if (txt  != null) slotField.stackText = txt;
        }
        slotField.SetItem(null, 0);
    }
}
