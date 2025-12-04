using UnityEngine;
using UnityEngine.EventSystems;

namespace WildsAdv
{
    public class RoomInteraction : MonoBehaviour, IPointerClickHandler
    {
        public string ObjectID;

        // Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Output to console the clicked GameObject's name and the following message.
            Debug.Log(ObjectID + " Game Object Clicked at " + pointerEventData);
        }
    }
}
