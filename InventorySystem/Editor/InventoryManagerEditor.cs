using UnityEditor;
using UnityEngine;

public class InventoryManagerEditor : InventoryManagerBaseEditor
{
    private enum Tab { CreateInventory, CreateItem, CreateQuickAccessBar, ItemDatabase }
    private Tab currentTab = Tab.CreateInventory;

    private CreateInventoryEditor createInventoryEditor;
    private CreateItemEditor createItemEditor;
    private CreateQuickAccessBarEditor createQuickAccessBarEditor;
    private ItemDatabaseEditor itemDatabaseEditor;

    [MenuItem("Inventory System/Inventory Manager")]
    public static void ShowWindow()
    {
        GetWindow<InventoryManagerEditor>("Inventory System");
    }

    private void OnEnable()
    {
        createInventoryEditor = new CreateInventoryEditor();
        createItemEditor = new CreateItemEditor();
        createQuickAccessBarEditor = new CreateQuickAccessBarEditor();
        itemDatabaseEditor = new ItemDatabaseEditor();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawTabs();
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 160), GUILayout.ExpandWidth(true));
        BeginScroll();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10); // Left padding
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
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(20); // Right padding
        EditorGUILayout.EndHorizontal();
        EndScroll();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTabs()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(150));
        GUILayout.Label("Inventory Manager", EditorStyles.boldLabel);
        if (GUILayout.Button("Create Inventory"))
        {
            currentTab = Tab.CreateInventory;
        }
        if (GUILayout.Button("Create Item"))
        {
            currentTab = Tab.CreateItem;
        }
        if (GUILayout.Button("Create Quick Access Bar"))
        {
            currentTab = Tab.CreateQuickAccessBar;
        }
        if (GUILayout.Button("Item Database"))
        {
            currentTab = Tab.ItemDatabase;
        }
        EditorGUILayout.EndVertical();
    }
}
