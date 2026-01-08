using UnityEngine;

namespace WildsAdv
{
    public class CloseBehavior : MonoBehaviour
    {
        public string tagToClose = "ItemCanvas";
        public bool transient = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public void OnClick()
        {
            GameObject objectToClose = GameObject.FindWithTag(tagToClose);
            if (objectToClose)
            {
                if (!transient)
                {
                    objectToClose.SetActive(false);
                }
                else
                {
                    Destroy(objectToClose);
                }
            }
            else
            {
                Debug.LogError("GameObject with tag " + tagToClose + " not found; could not close it.");
            }
        }
    }
}
