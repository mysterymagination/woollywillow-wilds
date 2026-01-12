using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace WildsAdv
{
    public class CameraZoomControl : MonoBehaviour
    {
        /// <summary>
        /// The rate at which the camera zooms in response to mouse scroll.
        /// </summary>
        public float zoomRate = 50.0F;

        // Update is called once per frame
        void Update()
        {
            // Apparently the scrollwheel vectorcontrol comes out as 1 in Y when scrolling in/up and -1 in Y when scrolling out/down. X appears unused? 
            InputAction scrollWheelAction = InputSystem.actions.FindAction("ScrollWheel");
            Vector2 scrollWheelChange = scrollWheelAction.ReadValue<Vector2>();
            Vector3 updatedCamPos = Camera.main.transform.position;
            updatedCamPos.z += scrollWheelChange[1] * zoomRate;
            Debug.Log("Scrollwheelchange says " + scrollWheelChange + " and new cam pos is " + updatedCamPos);
            Camera.main.transform.position = updatedCamPos;
        }
    }
}
