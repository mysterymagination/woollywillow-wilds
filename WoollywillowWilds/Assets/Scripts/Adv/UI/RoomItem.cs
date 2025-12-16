using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WildsAdv
{
    public class RoomInteraction : MonoBehaviour, IPointerClickHandler
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

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);

            GameObject roomView = GameObject.FindWithTag("RoomView");
            // load the ${objectID}_ItemDetail image into the detailPanelPrefab's ItemPanel -> ItemImage imageview. 
            if (roomView && detailPanelPrefab)
            {
                // Add image texture2d to item detail panel.
                // TODO: why is the instantiated subcanvas not rendering over the main canvas?
                GameObject detailPanel = Instantiate(detailPanelPrefab, roomView.transform.position, roomView.transform.rotation, roomView.transform);//new Vector3(0.0F, 0.0F, 0.0F), Quaternion.identity);
                /*
                GameObject itemImage = detailPanel.transform.Find("ItemPanel").Find("ItemImage").gameObject;
                Image image = itemImage.GetComponent<Image>();
                image.sprite = detailImage;
                */

            }
            // TODO: load the GenerateItemDescription() result into StoryText. 
        }
    }
}
