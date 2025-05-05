using UnityEngine;
using UnityEngine.UI;

namespace Nakshatra.InventorySystem
{
    public class ItemDescriptionPanel : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        public Image   iconImage;
        public Text    nameText;
        public Text    descText;
        public Transform statsParent;
        public GameObject statLinePrefab;  // a small prefab with two Texts: stat name + value

        public void Initialize()
        {
            // set up a simple layout:
            var rt = GetComponent<RectTransform>();
            var layout = gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8,8,8,8);
            layout.spacing = 4;

            // Icon
            var iconGO = new GameObject("Icon", typeof(Image));
            iconGO.transform.SetParent(transform, false);
            iconImage = iconGO.GetComponent<Image>();
            iconImage.rectTransform.sizeDelta = new Vector2(64,64);

            // Name
            var nameGO = new GameObject("Name", typeof(Text));
            nameGO.transform.SetParent(transform, false);
            nameText = nameGO.GetComponent<Text>();
            nameText.fontSize = 18;
            nameText.alignment = TextAnchor.MiddleCenter;

            // Description
            var descGO = new GameObject("Description", typeof(Text));
            descGO.transform.SetParent(transform, false);
            descText = descGO.GetComponent<Text>();
            descText.fontSize = 14;
            descText.alignment = TextAnchor.UpperLeft;
            descText.horizontalOverflow = HorizontalWrapMode.Wrap;

            // Stats Container
            var statsGO = new GameObject("Stats", typeof(VerticalLayoutGroup));
            statsGO.transform.SetParent(transform, false);
            statsParent = statsGO.transform;

            // Hide at start
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        /// <summary> Populates panel from a given InventoryItem. </summary>
        public void Show(InventoryItem item)
        {
            if (item == null) { Hide(); return; }

            iconImage.sprite     = item.itemIcon;
            nameText.text        = item.itemName;
            descText.text        = item.itemDescription;

            // clear old stats
            foreach (Transform c in statsParent) Destroy(c.gameObject);

            // add new stats
            foreach (var stat in item.stats)
            {
                var line = Instantiate(statLinePrefab, statsParent);
                var texts = line.GetComponentsInChildren<Text>();
                texts[0].text = stat.statType.ToString();
                texts[1].text = stat.value.ToString();
            }

            canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
        }
    }

}
