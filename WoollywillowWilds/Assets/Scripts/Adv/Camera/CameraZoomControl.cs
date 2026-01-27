using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System;
using Microsoft.Unity.VisualStudio.Editor;


namespace WildsAdv
{
    /// <summary>
    /// Installs main camera zoom behavior over the RoomView canvas. Relies on the root canvas render mode to be Projection.
    /// </summary>
    public class CameraZoomControl : MonoBehaviour
    {
        /// <summary>
        /// The rate at which the camera zooms in response to mouse scroll.
        /// </summary>
        public float zoomRate = 50.0F;
        public float magnificationFactor = 2.0F;

        // Update is called once per frame
        void Update()
        {
            /* camera/virtual camera attempts; looking complicated at best. Assumes perspective projection cam and worldspace canvas, neither or which are great for 2D.
            // we need two data points:
            //   1. Does RoomView have pointer focus? We need this in order to see if scroll to zoom should be active at all.
            //      EDIT: we can use the value from #2 to see if the world space pointer location is inside the roomview world space rect bounds. Would be nice to just have an onfocusgained, onfocuslost event...
            //      UI Toolkit (UIElements namespace) has some promising looking stuff, but I already ditched that for uGUI.
            //   2. Where in world space is the mouse pointer? It'll be screen space initially, and we want world space so we can translate the camera x,y prior to modifying camera z and zooming.
            //      EDIT: looks like we can sort of do this with Camera.ScreenToWorldPoint(), although X,y parity is reversed relative to the world space roomview corners given by roomRectTransform.GetWorldCorners().


            InputAction pointAction = InputSystem.actions.FindAction("Point");
            Vector2 pointerPos = pointAction.ReadValue<Vector2>();
            Vector3 worldPointerPos = Camera.main.ScreenToWorldPoint(new Vector3(pointerPos.x, pointerPos.y, Math.Abs(Camera.main.transform.position.z)));
            Debug.Log("Pointer loc in screen space is " + pointerPos + ", and in world space is " + worldPointerPos);

            // ImGUI stuff? idk, doesn't work in uGUI.
            //string focus = GUI.GetNameOfFocusedControl();
            //Debug.Log("Focused control name is " + focus);

            // Deprecated and no longer works in 6000.2
            //GameObject currentFocus = EventSystem.current.currentSelectedGameObject;
            //Debug.Log("Focused go is " + currentFocus);

            GameObject parentView = GameObject.FindWithTag("RoomView");
            RectTransform roomRectTransform = parentView.GetComponent<RectTransform>();
            Vector3[] roomCorners = new Vector3[4];
            roomRectTransform.GetWorldCorners(roomCorners);
            Debug.Log("Room corners are " + new StringBuilder()
                          .AppendJoin(", ", roomCorners).ToString());

            // Confusing and weird, tries to start top left corner with x and y increasing right + down. Nothing else works that way?
            //Rect roomBounds = roomRectTransform.rect;
            //Debug.Log("Room bound rect says " + roomBounds.ToString());

            if (worldPointerPos.x >= roomCorners[0].x && worldPointerPos.y >= roomCorners[0].y
                && worldPointerPos.x <= roomCorners[2].x && worldPointerPos.y <= roomCorners[2].y)
            {
                // Apparently the scrollwheel vectorcontrol comes out as 1 in Y when scrolling in/up and -1 in Y when scrolling out/down. X appears unused? 
                InputAction scrollWheelAction = InputSystem.actions.FindAction("ScrollWheel");
                Vector2 scrollWheelChange = scrollWheelAction.ReadValue<Vector2>();
                Vector3 updatedCamPos = Camera.main.transform.position;
                // todo: This technically kind of works, but the experience sucks. Better to make the translation part explicit, like the user right clicks someplace and that becomes camera x,y and then scroll only changes z.
                //if (scrollWheelChange[1] != 0.0F)
                //{
                //    updatedCamPos.x = worldPointerPos.x;
                //    updatedCamPos.y = worldPointerPos.y;
                //}
                
                updatedCamPos.z += scrollWheelChange[1] * zoomRate;
                Debug.Log("Scrollwheelchange says " + scrollWheelChange + " and new cam pos is " + updatedCamPos);
                Camera.main.transform.position = updatedCamPos;
            }
            */

            /* obviously not a great idea, and it only affects the story text for some reason.
            InputAction scrollWheelAction = InputSystem.actions.FindAction("ScrollWheel");
            Vector2 scrollWheelChange = scrollWheelAction.ReadValue<Vector2>();
            //Image image = gameObject.GetComponent<Image>();
            GameObject roomCanvasObject = GameObject.FindWithTag("RoomCanvas");
            Canvas roomCanvas = roomCanvasObject.GetComponent<Canvas>();
            roomCanvas.scaleFactor += magnificationFactor * scrollWheelChange[1];
            */
        }
    }
}
