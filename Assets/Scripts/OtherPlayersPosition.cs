using UnityEngine;
using UnityEngine.UI;

namespace Bray
{
    public class OtherPlayersPosition : MonoBehaviour
    {
        public Image top;
        public Image left;
        public Image right;
        public GameObject localPlayer;

        private void Awake()
        {
            if (top == null || left == null || right == null || localPlayer == null)
            {
                Debug.LogError("One or more Image or GameObject references are missing.");
                return;
            }
            Sprite sprite = Resources.Load<Sprite>("avatar");
            if (sprite == null)
            {
                Debug.LogError("Failed to load sprite from Resources/avatar");
                return;
            }
            top.sprite = sprite;
            left.sprite = sprite;
            right.sprite = sprite;
            SetImageSizeAndPosition(top, new Vector2(0.5f, 0.9f), new Vector2(0.07f, 0.12f));
            SetImageSizeAndPosition(left, new Vector2(0.1f, 0.5f), new Vector2(0.07f, 0.12f));
            SetImageSizeAndPosition(right, new Vector2(0.9f, 0.5f), new Vector2(0.07f, 0.12f));
            SetLocalPlayerPosition(localPlayer, new Vector2(0.5f, 0.15f), new Vector2(1800, 200));
        }
        private void SetImageSizeAndPosition(Image image, Vector2 anchorPosition, Vector2 sizePercentage)
        {
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = anchorPosition;
                rectTransform.anchorMax = anchorPosition;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                float width = sizePercentage.x * Screen.width;
                float height = sizePercentage.y * Screen.height;
                rectTransform.sizeDelta = new Vector2(width, height);
            }
            else
            {
                Debug.LogError("RectTransform not found on the Image component.");
            }
        }
        private void SetLocalPlayerPosition(GameObject localPlayer, Vector2 anchorPosition, Vector2 sizePercentage)
        {
            RectTransform rectTransform = localPlayer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = anchorPosition;
                rectTransform.anchorMax = anchorPosition;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                float width = sizePercentage.x;
                float height = sizePercentage.y;
                rectTransform.sizeDelta = new Vector2(width, height);
            }
            else
            {
                Debug.LogError("RectTransform not found on the localPlayer GameObject.");
            }
        }
    }
}