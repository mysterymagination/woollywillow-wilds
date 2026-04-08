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
        public TreasureText defaultDescription;
        /// <summary>
        /// An annotated script to be parsed by TreasureText.ParseAnnotatedText() which will be used in OnStart() to populate TextToTypeWrite.Contents if Contents is empty.
        /// </summary>
        public TextAsset descriptionTextAsset;
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
            TreasureText desc = GenerateDescription();
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
                    storyText.text = desc.ToString();
                }
            }
            else
            {
                Debug.LogError("Missing TMP_Text Component expected to display item description.");
            }
        }
        public TreasureText GenerateDescription()
        {
            if (defaultDescription.Contents.Count == 0 && descriptionTextAsset != null && !string.IsNullOrEmpty(descriptionTextAsset.text))
            {
                Debug.Log("Parsing " + descriptionTextAsset.name + " for our treasuretext contents.");
                defaultDescription.ParseAnnotatedText(descriptionTextAsset.text);
            }
            return defaultDescription;
        }
    }
}
