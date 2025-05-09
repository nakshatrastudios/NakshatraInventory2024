using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Nakshatra.InventorySystem.Editor
{
    public class CreateInventoryEditor : EditorWindow
    {
        private int numSlots;
        private int slotsPerRow;
        private int rowsPerPage;
        private Sprite backgroundSprite;
        private GameObject slotPrefab;
        private GameObject nextPageButtonPrefab;
        private GameObject previousPageButtonPrefab;
        private GameObject hpBarPrefab;
        private GameObject manaBarPrefab;
        private GameObject staminaBarPrefab;
        private int paddingLeft;
        private int paddingRight;
        private int paddingTop;
        private int paddingBottom;
        private Vector2 gridSpacing;
        private Vector2 cellSize;
        private float backgroundPaddingPercentage = 17f;

        private List<Currency> currencies = new List<Currency>();
        private Dictionary<string, int> currencyAmounts = new Dictionary<string, int>();

        
        public void OnGUI()
        {
            GUILayout.Label("Inventory Settings", EditorStyles.boldLabel);
            numSlots = EditorGUILayout.IntField("Number of Slots", numSlots);
            slotsPerRow = EditorGUILayout.IntField("Slots Per Row", slotsPerRow);
            rowsPerPage = EditorGUILayout.IntField("Rows Per Page", rowsPerPage);
            backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", backgroundSprite, typeof(Sprite), false);
            slotPrefab = (GameObject)EditorGUILayout.ObjectField("Slot Prefab", slotPrefab, typeof(GameObject), false);
            nextPageButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Next Page Button Prefab", nextPageButtonPrefab, typeof(GameObject), false);
            previousPageButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Previous Page Button Prefab", previousPageButtonPrefab, typeof(GameObject), false);

            GUILayout.Label("Grid Layout Settings", EditorStyles.boldLabel);
            paddingLeft = EditorGUILayout.IntField("Padding Left", paddingLeft);
            paddingRight = EditorGUILayout.IntField("Padding Right", paddingRight);
            paddingTop = EditorGUILayout.IntField("Padding Top", paddingTop);
            paddingBottom = EditorGUILayout.IntField("Padding Bottom", paddingBottom);
            gridSpacing = EditorGUILayout.Vector2Field("Grid Spacing", gridSpacing);
            cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);

            GUILayout.Label("Background Padding Percentage", EditorStyles.boldLabel);
            backgroundPaddingPercentage = EditorGUILayout.Slider("Padding Percentage", backgroundPaddingPercentage, 0, 20);

            GUILayout.Label("HUD Settings", EditorStyles.boldLabel);
            hpBarPrefab = (GameObject)EditorGUILayout.ObjectField("HP Bar Prefab", hpBarPrefab, typeof(GameObject), false);
            manaBarPrefab = (GameObject)EditorGUILayout.ObjectField("Mana Bar Prefab", manaBarPrefab, typeof(GameObject), false);
            staminaBarPrefab = (GameObject)EditorGUILayout.ObjectField("Stamina Bar Prefab", staminaBarPrefab, typeof(GameObject), false);

            GUILayout.Label("Currency Settings", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Currency"))
            {
                var newCurrency = new Currency("New Currency", null, 1);
                currencies.Add(newCurrency);
                currencyAmounts[newCurrency.name] = 0;
            }

            for (int i = 0; i < currencies.Count; i++)
            {
                GUILayout.BeginHorizontal();
                currencies[i].name = EditorGUILayout.TextField("Name", currencies[i].name);
                currencies[i].icon = (Sprite)EditorGUILayout.ObjectField("Icon", currencies[i].icon, typeof(Sprite), false);
                currencies[i].conversionRate = EditorGUILayout.IntField("Conversion Rate", currencies[i].conversionRate);
                if (GUILayout.Button("Remove"))
                {
                    currencyAmounts.Remove(currencies[i].name);
                    currencies.RemoveAt(i);
                    i--; // Adjust index after removal
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Create Inventory"))
            {
                CreateInventory();
            }
        }

        private void CreateInventory()
        {
            EditorUtilities.EnsureEventSystem();
            // Find the player GameObject
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject with tag 'Player' not found.");
                return;
            }

            // Add Inventory script to the player GameObject
            Inventory inventoryComponent = player.AddComponent<Inventory>();

            // Add CurrencyManager script to the player GameObject
            CurrencyManager currencyManager = player.AddComponent<CurrencyManager>();
            foreach (var currency in currencies)
            {
                currencyManager.currencies.Add(new CurrencyManager.Currency
                {
                    name = currency.name,
                    icon = currency.icon,
                    amount = 0
                });
            }

            // Add PlayerStatus script to the player GameObject
            PlayerStatus playerStatus = player.AddComponent<PlayerStatus>();

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
                nextPageButtonRect.anchorMin = new Vector2(1, 0);
                nextPageButtonRect.anchorMax = new Vector2(1, 0);
                nextPageButtonRect.pivot = new Vector2(0.5f, 0.5f);
                nextPageButtonRect.anchoredPosition = new Vector2(-40, -25); // Adjust as needed
            }

            if (previousPageButtonPrefab != null)
            {
                previousPageButtonObject = Instantiate(previousPageButtonPrefab, inventoryObject.transform);
                RectTransform previousPageButtonRect = previousPageButtonObject.GetComponent<RectTransform>();
                previousPageButtonRect.anchorMin = new Vector2(1, 0);
                previousPageButtonRect.anchorMax = new Vector2(1, 0);
                previousPageButtonRect.pivot = new Vector2(0.5f, 0.5f);
                previousPageButtonRect.anchoredPosition = new Vector2(-100, -25); // Adjust as needed
            }

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

            // Create the HUD GameObject
            GameObject hudObject = new GameObject("HUD");
            hudObject.transform.SetParent(canvasObject.transform);
            RectTransform hudRectTransform = hudObject.AddComponent<RectTransform>();
            hudRectTransform.anchorMin = new Vector2(0, 0);
            hudRectTransform.anchorMax = new Vector2(0, 0);
            hudRectTransform.pivot = new Vector2(0, 0);
            hudRectTransform.anchoredPosition = new Vector2(10, 10); // Adjust as needed

            // Instantiate and set up the HP bar, Mana bar, and Stamina bar
            GameObject hpBar = null;
            GameObject manaBar = null;
            GameObject staminaBar = null;

            if (hpBarPrefab != null)
            {
                hpBar = Instantiate(hpBarPrefab, hudObject.transform);
                RectTransform hpBarRect = hpBar.GetComponent<RectTransform>();
                hpBarRect.anchorMin = new Vector2(0, 1);
                hpBarRect.anchorMax = new Vector2(0, 1);
                hpBarRect.pivot = new Vector2(0, 1);
                hpBarRect.anchoredPosition = new Vector2(10, -10); // Adjust as needed
            }

            if (manaBarPrefab != null)
            {
                manaBar = Instantiate(manaBarPrefab, hudObject.transform);
                RectTransform manaBarRect = manaBar.GetComponent<RectTransform>();
                manaBarRect.anchorMin = new Vector2(0, 1);
                manaBarRect.anchorMax = new Vector2(0, 1);
                manaBarRect.pivot = new Vector2(0, 1);
                manaBarRect.anchoredPosition = new Vector2(10, -40); // Adjust as needed
            }

            if (staminaBarPrefab != null)
            {
                staminaBar = Instantiate(staminaBarPrefab, hudObject.transform);
                RectTransform staminaBarRect = staminaBar.GetComponent<RectTransform>();
                staminaBarRect.anchorMin = new Vector2(0, 1);
                staminaBarRect.anchorMax = new Vector2(0, 1);
                staminaBarRect.pivot = new Vector2(0, 1);
                staminaBarRect.anchoredPosition = new Vector2(10, -70); // Adjust as needed
            }

            // Add HUD script to the HUD GameObject and set references
            HUD hudComponent = hudObject.AddComponent<HUD>();
            if (hpBar != null) hudComponent.healthBarFill = hpBar.transform.GetChild(0).GetComponent<Image>();
            if (manaBar != null) hudComponent.manaBarFill = manaBar.transform.GetChild(0).GetComponent<Image>();
            if (staminaBar != null) hudComponent.staminaBarFill = staminaBar.transform.GetChild(0).GetComponent<Image>();

            // Create the Currency Display GameObject
            GameObject currencyDisplayObject = new GameObject("CurrencyDisplay");
            currencyDisplayObject.transform.SetParent(inventoryObject.transform);
            RectTransform currencyDisplayRect = currencyDisplayObject.AddComponent<RectTransform>();
            currencyDisplayRect.anchorMin = new Vector2(0, 0);
            currencyDisplayRect.anchorMax = new Vector2(0, 0);
            currencyDisplayRect.pivot = new Vector2(0, 1);
            currencyDisplayRect.anchoredPosition = new Vector2(10, 0); // Adjust as needed
            currencyDisplayRect.sizeDelta = new Vector2(300, 50);

            HorizontalLayoutGroup currencyLayout = currencyDisplayObject.AddComponent<HorizontalLayoutGroup>();
            currencyLayout.childAlignment = TextAnchor.UpperRight;
            currencyLayout.spacing = 50;
            currencyLayout.childControlHeight = true;
            currencyLayout.childControlWidth = true;
            currencyLayout.reverseArrangement = true;

            // Instantiate and set up currency icons and amounts
            foreach (var currency in currencies)
            {
                GameObject currencyObject = new GameObject(currency.name + "Icon");
                currencyObject.transform.SetParent(currencyDisplayObject.transform);

                Image currencyImage = currencyObject.AddComponent<Image>();
                currencyImage.sprite = currency.icon;

                Text currencyText = new GameObject(currency.name + "Text").AddComponent<Text>();
                currencyText.transform.SetParent(currencyObject.transform);
                currencyText.text = "0";
                currencyText.alignment = TextAnchor.LowerLeft;
                currencyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                currencyText.fontSize = 20;
                RectTransform currencyTextRect = currencyText.GetComponent<RectTransform>();
                currencyTextRect.anchorMin = new Vector2(1, 0);
                currencyTextRect.anchorMax = new Vector2(1, 0);
                currencyTextRect.pivot = new Vector2(0, 0);
                currencyTextRect.sizeDelta = new Vector2(50, 25);
                currencyTextRect.anchoredPosition = new Vector2(0, 0);

                // Reference the currency text in the CurrencyManager
                var currencyManagerCurrency = currencyManager.currencies.Find(c => c.name == currency.name);
                if (currencyManagerCurrency != null)
                {
                    currencyManagerCurrency.currencyText = currencyText;
                }
            }

            Debug.Log("Inventory, HUD, and Currency system created successfully.");
        }

        [System.Serializable]
        public class Currency
        {
            public string name;
            public Sprite icon;
            public int conversionRate;

            public Currency(string name, Sprite icon, int conversionRate)
            {
                this.name = name;
                this.icon = icon;
                this.conversionRate = conversionRate;
            }
        }
    }
}