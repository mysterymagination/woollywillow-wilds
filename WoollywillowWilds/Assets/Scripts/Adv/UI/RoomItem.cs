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
         * TODO: is this the right way to instantiate a given prefab? I thought GameObjects had to be active in a scene? It seems like an asset path string or similar would make more sense.
         */
        public GameObject detailPanelPrefab;

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);

            // TODO: load the ${objectID}_ItemDetail image into the detailPanelPrefab's ItemPanel -> ItemImage imageview. 
            // TODO: load the GenerateItemDescription() result into StoryText. 
        }
    }
}
