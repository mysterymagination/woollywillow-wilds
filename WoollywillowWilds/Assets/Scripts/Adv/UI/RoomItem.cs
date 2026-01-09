using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WildsAdv
{
    public class RoomItem : MonoBehaviour, IPointerClickHandler, IInteractHandler
    {
        /**
         * Human readable object name assigned in the editor.
         */
        public string objectID;
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
            // TODO: add direct list of event listeners for inspector or just lean on Button that already does so, then loop through and callback to 'em

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
