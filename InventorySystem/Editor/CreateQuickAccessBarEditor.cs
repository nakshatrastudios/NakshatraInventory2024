using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Nakshatra.InventorySystem.Editor
{
    public class CreateQuickAccessBarEditor : InventoryManagerBaseEditor
    {
        private int quickAccessSlots;
        private Sprite quickAccessBackgroundSprite;
        private GameObject quickAccessSlotPrefab;
        private GameObject buttonNumberTextPrefab; // New field for button number text prefab
        private int quickAccessPaddingLeft;
        private int quickAccessPaddingRight;
        private int quickAccessPaddingTop;
        private int quickAccessPaddingBottom;
        private Vector2 quickAccessGridSpacing;
        private Vector2 quickAccessCellSize;
        private float quickAccessBackgroundPaddingPercentage = 17f;

        public void DrawCreateQuickAccessBar()
        {
            GUILayout.Label("Quick Access Bar Settings", EditorStyles.boldLabel);
            quickAccessSlots = EditorGUILayout.IntField("Number of Slots", quickAccessSlots);
            quickAccessSlots = Mathf.Clamp(quickAccessSlots, 1, 10);
            quickAccessBackgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", quickAccessBackgroundSprite, typeof(Sprite), false);
            quickAccessSlotPrefab = (GameObject)EditorGUILayout.ObjectField("Slot Prefab", quickAccessSlotPrefab, typeof(GameObject), false);
            buttonNumberTextPrefab = (GameObject)EditorGUILayout.ObjectField("Button Number Text Prefab", buttonNumberTextPrefab, typeof(GameObject), false);

            GUILayout.Label("Grid Layout Settings", EditorStyles.boldLabel);
            quickAccessPaddingLeft = EditorGUILayout.IntField("Padding Left", quickAccessPaddingLeft);
            quickAccessPaddingRight = EditorGUILayout.IntField("Padding Right", quickAccessPaddingRight);
            quickAccessPaddingTop = EditorGUILayout.IntField("Padding Top", quickAccessPaddingTop);
            quickAccessPaddingBottom = EditorGUILayout.IntField("Padding Bottom", quickAccessPaddingBottom);
            quickAccessGridSpacing = EditorGUILayout.Vector2Field("Grid Spacing", quickAccessGridSpacing);
            quickAccessCellSize = EditorGUILayout.Vector2Field("Cell Size", quickAccessCellSize);

            GUILayout.Label("Background Padding Percentage", EditorStyles.boldLabel);
            quickAccessBackgroundPaddingPercentage = EditorGUILayout.Slider("Padding Percentage", quickAccessBackgroundPaddingPercentage, 0, 20);

            if (GUILayout.Button("Create Quick Access Bar"))
            {
                CreateQuickAccessBar();
            }
        }

        private void CreateQuickAccessBar()
        {
            // Find the player GameObject
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject with tag 'Player' not found.");
                return;
            }

            // Add QuickAccessBar script to the player GameObject
            QuickAccessBar quickAccessBarComponent = player.AddComponent<QuickAccessBar>();

            // Create a Canvas
            GameObject canvasObject = new GameObject("QuickAccessCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasObject.AddComponent<GraphicRaycaster>();

            // Create the Quick Access Bar GameObject
            GameObject quickAccessBarObject = new GameObject("QuickAccessBar");
            quickAccessBarObject.transform.SetParent(canvasObject.transform);
            quickAccessBarObject.transform.localPosition = Vector3.zero;

            // Set background sprite if provided
            if (quickAccessBackgroundSprite != null)
            {
                Image backgroundImage = quickAccessBarObject.AddComponent<Image>();
                backgroundImage.sprite = quickAccessBackgroundSprite;
                RectTransform rectTransform = quickAccessBarObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                rectTransform.anchoredPosition = new Vector2(10, 10);
            }

            // Create the QuickAccessGrid GameObject
            GameObject quickAccessGridObject = new GameObject("QuickAccessGrid");
            quickAccessGridObject.transform.SetParent(quickAccessBarObject.transform);
            GridLayoutGroup gridLayout = quickAccessGridObject.AddComponent<GridLayoutGroup>();
            RectTransform gridRectTransform = quickAccessGridObject.GetComponent<RectTransform>();
            gridRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            gridRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            gridRectTransform.pivot = new Vector2(0.5f, 0.5f);
            gridRectTransform.anchoredPosition = new Vector2(0, -quickAccessPaddingTop);

            // Set padding, spacing, and cell size for the GridLayoutGroup
            gridLayout.padding = new RectOffset(quickAccessPaddingLeft, quickAccessPaddingRight, quickAccessPaddingTop, quickAccessPaddingBottom);
            gridLayout.spacing = quickAccessGridSpacing;
            gridLayout.cellSize = quickAccessCellSize;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = quickAccessSlots;

            // Calculate the total size of the grid
            float gridWidth = quickAccessCellSize.x * quickAccessSlots + quickAccessGridSpacing.x * (quickAccessSlots - 1) + quickAccessPaddingLeft + quickAccessPaddingRight;
            float gridHeight = quickAccessCellSize.y + quickAccessGridSpacing.y + quickAccessPaddingTop + quickAccessPaddingBottom;

            // Set the size of the grid RectTransform
            gridRectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

            // Set the size of the quick access bar background RectTransform to be based on the padding percentage
            RectTransform quickAccessRectTransform = quickAccessBarObject.GetComponent<RectTransform>();
            float paddingFactor = 1 + quickAccessBackgroundPaddingPercentage / 100;
            quickAccessRectTransform.sizeDelta = new Vector2(gridWidth * paddingFactor, gridHeight * paddingFactor);

            // Assign references in the QuickAccessBar component
            quickAccessBarComponent.totalSlots = quickAccessSlots;
            quickAccessBarComponent.slotPrefab = quickAccessSlotPrefab;
            quickAccessBarComponent.quickAccessGrid = quickAccessGridObject.transform;
            quickAccessBarComponent.buttonNumberTextPrefab = buttonNumberTextPrefab; // Assign the button number text prefab

            Debug.Log("Quick Access Bar created successfully.");
        }
    }
}