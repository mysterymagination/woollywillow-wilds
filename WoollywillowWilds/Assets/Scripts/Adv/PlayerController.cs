using UnityEngine;
using UnityEngine.InputSystem;

namespace WildsAdv
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 0.05f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float horizontal_dir = 0.0f;
            if (Keyboard.current.leftArrowKey.isPressed)
            {

                horizontal_dir = -1.0f;

            }
            else if (Keyboard.current.rightArrowKey.isPressed)
            {

                horizontal_dir = 1.0f;

            }

            float vertical_dir = 0.0f;
            if (Keyboard.current.downArrowKey.isPressed)
            {

                vertical_dir = -1.0f;

            }
            else if (Keyboard.current.upArrowKey.isPressed)
            {

                vertical_dir = 1.0f;

            }
            Debug.Log("The horizontal,vertical directions say: " + horizontal_dir + "," + vertical_dir);


            Vector2 position = transform.position;
            position.x = position.x + speed * horizontal_dir;
            position.y = position.y + speed * vertical_dir;
            transform.position = position;
        }
    }
}