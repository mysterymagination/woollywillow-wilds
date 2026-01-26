using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
        /**
         * Simple no-arg UnityEvent that will Invoke() in OnInteract(), allowing any registered display handling that does not rely on input data to run.
         */
        public UnityEvent DisplayHandler;
        public delegate int Calculate(int x, int y);

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);
            OnInteract(pointerEventData);
        }
        public void OnInteract(PointerEventData pointerEventData)
        {
            // Handle any display we want e.g. loading up canvas with detailed image.
            DisplayHandler.Invoke();

            // Load the GenerateItemDescription() result into StoryText.
            string desc = GenerateDescription();
            GameObject storyTextObject = GameObject.FindWithTag("StoryText");
            TMP_Text storyText = storyTextObject.GetComponent<TMP_Text>();
            TypeWriter writer = storyTextObject.GetComponent<TypeWriter>();
            if (storyText)
            {
                storyText.text = "";
                if (writer)
                {
                    writer.ResetState();
                    writer.TextToTypeWrite = desc;
                    writer.TypeWrite();
                }
                else
                {
                    storyText.text = desc;
                }
            }
            else
            {
                Debug.LogError("Missing TMP_Text Component expected to display item description.");
            }
        }
        public string GenerateDescription()
        {
            return defaultDescription;
        }
    }
}
