using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WildsAdv
{
    public class RoomInteractable_Audio : MonoBehaviour, IPointerClickHandler
    {
        /**
         * Human readable object name assigned in the editor.
         */
        public string objectID;

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(objectID + " Game Object Clicked at " + pointerEventData);
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.Play();
            }
        }
    }
}
