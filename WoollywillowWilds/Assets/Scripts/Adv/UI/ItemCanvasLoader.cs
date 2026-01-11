using System;
using UnityEngine;
using UnityEngine.UI;

namespace WildsAdv
{
    public class ItemCanvasLoader : MonoBehaviour
    {
        /**
            <summary>
                A GameObject in the scene or prefab asset with a UI canvas hosting an image view ready to host the detail image.
            </summary>
         */
        public GameObject detailCanvas;
        /**
            <summary>
                A GameObject with texture data representing the item detail image.
            </summary>
         */
        public Sprite detailImage;
        /**
            <summary>
                Tag of the UI container parent of the loaded Canvas.
            </summary>
         */
        public string canvasParentTag = "RoomView";
        /**
            <summary>
                Tag of the item Panel UI element that contains our Image UI element.
            </summary>
         */
        public string imageParentTag = "ItemPanel";
        /**
            <summary>
                Tag of the item Image UI element into which we load our detailImage Sprite.
            </summary>
         */
        public string imageTag = "ItemImage";
        /**
            <summary>
                Tag of the UI element which should close the canvas onclick.
            </summary>
         */
        public string closeTag = "ExitButton";
        /**
            <summary>
                True if the Canvas given in detailPanel needs to be instantiated from a prefab, false if the Canvas is already-extant part of the scene and simply needs to be activated.
            </summary>
         */
        public bool isTransient = false;
        /**
            <summary>
                Handle to the loaded display Canvas.
            </summary>
         */
        private GameObject loadedCanvas;

        public void CanvasLoadHandler()
        {
            GameObject parentView = GameObject.FindWithTag(canvasParentTag);
            // load the detail image into the detailPanelPrefab's ItemPanel -> ItemImage imageview. 
            if (parentView && detailCanvas)
            {
                if (isTransient)
                {
                    // Load item detail panel prefab as relative transform child of the given parent.
                    loadedCanvas = Instantiate(detailCanvas, parentView.transform, false);
                }
                else
                {
                    loadedCanvas = detailCanvas;
                    loadedCanvas.SetActive(true);
                }

                // Search for the configured image hierarchy.
                GameObject itemImage;
                try
                {
                    itemImage = loadedCanvas.transform.Find(imageParentTag).Find(imageTag).gameObject;
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("Failed finding " + imageParentTag + " transform child of loaded canvas and/or its " + imageTag + " transform child.");
                    throw e;
                }

                // Load the item detail image.
                Image image = itemImage.GetComponent<Image>();
                image.sprite = detailImage;

                // Install exit/unload behavior
                GameObject exitButtonGameObject;
                try
                {
                    exitButtonGameObject = loadedCanvas.transform.Find(closeTag).gameObject;
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("Failed finding " + closeTag + " transform child of loaded canvas.");
                    throw e;
                }
                Button exitButton = exitButtonGameObject.GetComponent<Button>();
                // todo: this is a hacky solution for avoiding accumulation of close listeners, all of which after the latest will find loadedCanvas already nullified.
                //   Better might be to limit this unload handler installation to transient canvasi and then assume the in-scene one has its unload already worked out?
                //   We could install the unloader behavior on the exitbutton of the in-scene FrameCanvas instance rather than the prefab to cover this.
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(CanvasUnloadHandler);
            }
            else
            {
                Debug.LogError("Failed to find parent view (" + parentView + ") or detailCanvas is unset (" + detailCanvas + ")");
            }
        }

        public void CanvasUnloadHandler()
        {
            if (loadedCanvas)
            {
                if (!isTransient)
                {
                    loadedCanvas.SetActive(false);
                }
                else
                {
                    Destroy(loadedCanvas);
                }
            }
            else
            {
                Debug.LogError("Loaded canvas is null; could not close it.");
            }
            loadedCanvas = null;
        }
    }
}
