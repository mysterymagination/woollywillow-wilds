using UnityEngine;
using UnityEngine.UI;

namespace WildsAdv
{
    public class ItemCanvasLoader : MonoBehaviour
    {
        /**
         * A GameObject in the scene or prefab asset with a UI panel and image view ready to host the detail image.
         */
        public GameObject detailPanel;
        /**
         * A GameObject with texture data representing the item detail image.
         */
        public Sprite detailImage;
        /**
         * Tag of the UI container parent of the loaded Canvas.
         */
        public string parentTag = "RoomView";
        /**
         * Tag of the panel within the loaded Canvas that serves as the direct container of the item image.
         */
        public string imageParentTag = "ItemPanel";
        /**
         * Tag of the item Image UI element into which we load our detailImage Sprite.
         */
        public string imageTag = "ItemImage";
        /**
         * Determines if the Canvas given in detailPanel needs to be instantiated from a prefab or simply activated. 
         */
        public bool isPrefab = false;

        public void CanvasLoadHandler()
        {
            GameObject parentView = GameObject.FindWithTag(parentTag);
            // load the detail image into the detailPanelPrefab's ItemPanel -> ItemImage imageview. 
            if (parentView && detailPanel)
            {
                GameObject loadedPanel;
                if (isPrefab)
                {
                    // Load item detail panel prefab as relative transform child of the given parent.
                    loadedPanel = Instantiate(detailPanel, parentView.transform, false);
                }
                else
                {
                    loadedPanel = detailPanel;
                    loadedPanel.SetActive(true);
                }
                GameObject itemImage = loadedPanel.transform.Find(imageParentTag).Find(imageTag).gameObject;
                // Load the item detail image.
                Image image = itemImage.GetComponent<Image>();
                image.sprite = detailImage;
            }
        }
    }
}
