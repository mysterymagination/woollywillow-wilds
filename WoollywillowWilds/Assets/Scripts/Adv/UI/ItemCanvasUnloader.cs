using UnityEngine;

namespace WildsAdv
{
    public class ItemCanvasUnloader : MonoBehaviour
    {
        /// <summary>
        /// Handle to the canvas we need to close.
        /// </summary>
        public GameObject canvasToClose;
        /// <summary>
        /// True if the canvas should be destroyed on close, false if the canvas should only be deactivated on close.
        /// </summary>
        public bool transient = false;

        /// <summary>
        /// Function suitable for use as a UnityAction registered with a UnityEvent that handles closing/unloading an item detail canvas.
        /// </summary>
        public void CanvasUnloadHandler()
        {
            if (canvasToClose)
            {
                if (!transient)
                {
                    canvasToClose.SetActive(false);
                }
                else
                {
                    Destroy(canvasToClose);
                }
                GameObject storyTextObject = GameObject.FindWithTag("StoryText");
                if (storyTextObject)
                {
                    storyTextObject.BroadcastMessage("OnCanvasClose");
                }
                else
                {
                    Debug.LogError("GameObject textview with tag StoryText not found; could not broadcast message OnCanvasClose.");
                }
            }
            else
            {
                Debug.LogError("GameObject " + canvasToClose + " not found; could not close it.");
            }
        }
    }
}
