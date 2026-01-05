using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WildsAdv
{
    public class RoomItem : MonoBehaviour, IPointerClickHandler, IInteractHandler
    {
        /**
         * Human readable object name assigned in the editor.
         */
        public string objectID;
        /**
         * A GameObject prefab with a UI panel and image view ready to host the detail image.
         */
        public GameObject detailPanelPrefab;
        /**
         * A GameObject with texture data representing the item detail image.
         */
        public Sprite detailImage;
        /**
         * Starting description.
         */
        public string defaultDescription;

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);
            OnInteract(pointerEventData);
        }
        public void OnInteract(PointerEventData pointerEventData)
        {
            GameObject roomView = GameObject.FindWithTag("RoomView");
            // load the ${objectID}_ItemDetail image into the detailPanelPrefab's ItemPanel -> ItemImage imageview. 
            if (roomView && detailPanelPrefab)
            {
                // Load item detail panel prefab as relative transform child of RoomView.
                GameObject detailPanel = Instantiate(detailPanelPrefab, roomView.transform, false);
                GameObject itemImage = detailPanel.transform.Find("ItemPanel").Find("ItemImage").gameObject;
                // Load the item detail image.
                Image image = itemImage.GetComponent<Image>();
                image.sprite = detailImage;
                

            }
            // Load the GenerateItemDescription() result into StoryText.
            GameObject storyTextObject = GameObject.FindWithTag("StoryText");
            TMP_Text storyText = storyTextObject.GetComponent<TMP_Text>();
            storyText.text = GenerateDescription();
        }
        public string GenerateDescription()
        {
            return defaultDescription;
        }
    }
}
