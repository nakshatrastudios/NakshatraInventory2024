using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystemEditor : EditorWindow
{
    private int numSlots;
    private int slotsPerRow;
    private int rowsPerPage;
    private Sprite backgroundSprite;
    private GameObject slotPrefab;
    private GameObject nextPageButtonPrefab;
    private GameObject previousPageButtonPrefab;
    //private GameObject pickupTextPrefab;
    private int paddingLeft;
    private int paddingRight;
    private int paddingTop;
    private int paddingBottom;
    private Vector2 gridSpacing;
    private Vector2 cellSize;
    private float backgroundPaddingPercentage = 10f;

    [MenuItem("Inventory System/Create Inventory")]
    public static void ShowWindow()
    {
        GetWindow<InventorySystemEditor>("Create Inventory");
    }

    private void OnEnable()
    {
        paddingLeft = 0;
        paddingRight = 0;
        paddingTop = 0;
        paddingBottom = 0;
        gridSpacing = new Vector2(0, 0);
        cellSize = new Vector2(100, 100);
    }

    private void OnGUI()
    {
        GUILayout.Label("Inventory Settings", EditorStyles.boldLabel);
        numSlots = EditorGUILayout.IntField("Number of Slots", numSlots);
        slotsPerRow = EditorGUILayout.IntField("Slots Per Row", slotsPerRow);
        rowsPerPage = EditorGUILayout.IntField("Rows Per Page", rowsPerPage);
        backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", backgroundSprite, typeof(Sprite), false);
        slotPrefab = (GameObject)EditorGUILayout.ObjectField("Slot Prefab", slotPrefab, typeof(GameObject), false);
        nextPageButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Next Page Button Prefab", nextPageButtonPrefab, typeof(GameObject), false);
        previousPageButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Previous Page Button Prefab", previousPageButtonPrefab, typeof(GameObject), false);
        //pickupTextPrefab = (GameObject)EditorGUILayout.ObjectField("Pickup Text Prefab", pickupTextPrefab, typeof(GameObject), false);

        GUILayout.Label("Grid Layout Settings", EditorStyles.boldLabel);
        paddingLeft = EditorGUILayout.IntField("Padding Left", paddingLeft);
        paddingRight = EditorGUILayout.IntField("Padding Right", paddingRight);
        paddingTop = EditorGUILayout.IntField("Padding Top", paddingTop);
        paddingBottom = EditorGUILayout.IntField("Padding Bottom", paddingBottom);
        gridSpacing = EditorGUILayout.Vector2Field("Grid Spacing", gridSpacing);
        cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);

        GUILayout.Label("Background Padding Percentage", EditorStyles.boldLabel);
        backgroundPaddingPercentage = EditorGUILayout.Slider("Padding Percentage", backgroundPaddingPercentage, 0, 20);

        if (GUILayout.Button("Create Inventory"))
        {
            CreateInventory();
        }
    }

    private void CreateInventory()
    {
        // Find the player GameObject
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
            return;
        }

        // Add Inventory script to the player GameObject
        Inventory inventoryComponent = player.AddComponent<Inventory>();

        // Create a Canvas
        GameObject canvasObject = new GameObject("InventoryCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();

        // Create the Inventory GameObject
        GameObject inventoryObject = new GameObject("Inventory");
        inventoryObject.transform.SetParent(canvasObject.transform);
        inventoryObject.transform.localPosition = Vector3.zero;

        // Set background sprite if provided
        if (backgroundSprite != null)
        {
            Image backgroundImage = inventoryObject.AddComponent<Image>();
            backgroundImage.sprite = backgroundSprite;
            RectTransform rectTransform = inventoryObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        // Create the InventoryGrid GameObject
        GameObject inventoryGridObject = new GameObject("InventoryGrid");
        inventoryGridObject.transform.SetParent(inventoryObject.transform);
        GridLayoutGroup gridLayout = inventoryGridObject.AddComponent<GridLayoutGroup>();
        RectTransform gridRectTransform = inventoryGridObject.GetComponent<RectTransform>();
        gridRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        gridRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        gridRectTransform.pivot = new Vector2(0.5f, 0.5f);
        gridRectTransform.anchoredPosition = new Vector2(0, -paddingTop);

        // Set padding, spacing, and cell size for the GridLayoutGroup
        gridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        gridLayout.spacing = gridSpacing;
        gridLayout.cellSize = cellSize;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = slotsPerRow;

        // Calculate the total size of the grid
        float gridWidth = cellSize.x * slotsPerRow + gridSpacing.x * (slotsPerRow - 1) + paddingLeft + paddingRight;
        float gridHeight = cellSize.y * rowsPerPage + gridSpacing.y * (rowsPerPage - 1) + paddingTop + paddingBottom;

        // Set the size of the grid RectTransform
        gridRectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

        // Set the size of the inventory background RectTransform to be based on the padding percentage
        RectTransform inventoryRectTransform = inventoryObject.GetComponent<RectTransform>();
        float paddingFactor = 1 + backgroundPaddingPercentage / 100;
        inventoryRectTransform.sizeDelta = new Vector2(gridWidth * paddingFactor, gridHeight * paddingFactor);

        // Positioning the navigation buttons
        GameObject nextPageButtonObject = null;
        GameObject previousPageButtonObject = null;

        if (nextPageButtonPrefab != null)
        {
            nextPageButtonObject = Instantiate(nextPageButtonPrefab, inventoryObject.transform);
            RectTransform nextPageButtonRect = nextPageButtonObject.GetComponent<RectTransform>();
            nextPageButtonRect.anchorMin = new Vector2(1, 0.5f);
            nextPageButtonRect.anchorMax = new Vector2(1, 0.5f);
            nextPageButtonRect.pivot = new Vector2(0.5f, 0.5f);
            nextPageButtonRect.anchoredPosition = new Vector2(50, 0); // Adjust as needed
        }

        if (previousPageButtonPrefab != null)
        {
            previousPageButtonObject = Instantiate(previousPageButtonPrefab, inventoryObject.transform);
            RectTransform previousPageButtonRect = previousPageButtonObject.GetComponent<RectTransform>();
            previousPageButtonRect.anchorMin = new Vector2(0, 0.5f);
            previousPageButtonRect.anchorMax = new Vector2(0, 0.5f);
            previousPageButtonRect.pivot = new Vector2(0.5f, 0.5f);
            previousPageButtonRect.anchoredPosition = new Vector2(-50, 0); // Adjust as needed
        }

        // Create and set the PickupText prefab inside the InventoryCanvas
        // if (pickupTextPrefab != null)
        // {
        //     GameObject pickupTextObject = Instantiate(pickupTextPrefab, canvasObject.transform);
        //     pickupTextObject.name = "PickupText";
        //     pickupTextObject.SetActive(false); // Hide initially
        //     inventoryComponent.pickupTextPrefab = pickupTextObject;
        // }

        // Assign references in the Inventory component
        inventoryComponent.totalSlots = numSlots;
        inventoryComponent.columns = slotsPerRow;
        inventoryComponent.rows = rowsPerPage;
        inventoryComponent.slotPrefab = slotPrefab;
        inventoryComponent.inventoryGrid = inventoryGridObject.transform;
        if (nextPageButtonObject != null) inventoryComponent.nextPageButton = nextPageButtonObject.GetComponent<Button>();
        if (previousPageButtonObject != null) inventoryComponent.previousPageButton = previousPageButtonObject.GetComponent<Button>();

        // Add InventoryPagination script to the Inventory GameObject
        InventoryPagination paginationComponent = inventoryObject.AddComponent<InventoryPagination>();
        paginationComponent.inventory = inventoryComponent;
        if (nextPageButtonObject != null) paginationComponent.nextPageButton = nextPageButtonObject.GetComponent<Button>();
        if (previousPageButtonObject != null) paginationComponent.previousPageButton = previousPageButtonObject.GetComponent<Button>();

        Debug.Log("Inventory created successfully.");
    }
}
