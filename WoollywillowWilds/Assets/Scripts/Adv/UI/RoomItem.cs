using UnityEngine;
using UnityEngine.EventSystems;

namespace WildsAdv
{
    public class RoomInteraction : MonoBehaviour, IPointerClickHandler
    {
        /**
         * Human readable object name assigned in the editor.
         */
        public string objectID;
        /**
         * A GameObject prefab with a UI panel and image view ready to host the <objectId>_DetailedImage sprite.
         */
        public GameObject detailPanelPrefab;

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);
        }
    }
}
