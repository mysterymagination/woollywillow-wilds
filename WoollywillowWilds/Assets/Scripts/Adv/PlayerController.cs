using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.Events;

namespace WildsAdv
{
    public class PlayerController : MonoBehaviour
    {
        /**
         * Determines whether we've moving in free acceleration or grid locked mode, gridSpeed cells at a time.
         */
        public bool gridMovementMode = true;
        /**
         * How many grid cells we traverse each move in grid movement mode.
         */
        public uint gridSpeed = 1;
        /**
         * Unity space unit length of a grid square side.
         */
        public uint gridSquareSide = 256;
        /**
        * Acceleration in m/s^2; acceleration is the rate of change of velocity, expressed
        * as meters per second per second as in velocity of N m/s increases by M m/s
        * every second. We'll use a constant rate of acceleration for simplicity,
        * but it might be neat to add a mechanic where holding the 'pedal' down
        * longer increases rate of acceleration.
        */
        public float acceleration = 0.0f;
        /**
         * Acceleration in m/s^2; acceleration is the rate of change of velocity, expressed
         * as meters per second per second as in velocity of N m/s increases by M m/s
         * every second. We'll use a constant rate of acceleration for simplicity,
         * but it might be neat to add a mechanic where holding the 'pedal' down
         * longer increases rate of acceleration.
         */
        public float deceleration = 0.0f;
        /**
         * Rate of change of acceleration in m/s^2, added to acceleration value per acceleration period with pedal held down.
         */
        public float accelerationRate = 2.0f;
        /**
         * Rate of change of deceleration in m/s^2, subtracted from acceleration value per deceleration period with pedal released.
         */
        public float decelerationRate = 2.0f;
        /**
         * Starting speed that increases/decreases with acceleration;
         * forward or backward (depth input axis) increases speed by 
         * acceleration over time, and releasing decreases speed by same.
         */
        public float speed = 0.0f;
        /**
         * Rotation speed in degrees/second.
         */
        public float turnSpeed = 55.0f;
        /**
         * Time period at which acceleration events occur in seconds.
         */
        public float accelerationEventPeriod = 1.0F;
        InputAction moveAction;
        UnityEvent accelerationEvent = new UnityEvent();
        /**
         * Tracks pedal pressed state in current acceleration period event.
         */
        private bool moving = false;
        /**
         * Tracks pedal pressed state from previous acceleration period event.
         */
        private bool previouslyMoving = false;
        /**
         * Tracks whether we are moving forward or backward by multiplying the forward vector by 1 or -1 based on forward or reverse input.
         */
        private float forwardDirectionFactor = 1.0F;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            moveAction.started += OnMoveInputStartEvent;
            moveAction.canceled += OnMoveInputEndEvent;

            accelerationEvent.AddListener(OnAccelerationPeriodEvent);
            ClockworkTasks clocks = gameObject.GetComponent<ClockworkTasks>();
            clocks.LaunchClock("AccelEvent", accelerationEvent, 0, true, accelerationEventPeriod);

        }

        // Update is called once per frame
        void Update()
        {
            /*
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
            */


            /*
            Vector2 position = transform.position;
            position.x = position.x + speed * horizontal_dir;
            position.y = position.y + speed * vertical_dir;
            transform.position = position;
            */

            // move player forward
            //transform.Translate(Time.deltaTime * speed * moveValue[0], Time.deltaTime * speed * moveValue[1]);
        }

        void OnMoveInputStartEvent(CallbackContext context)
        {
            Vector2 moveValue = context.action.ReadValue<Vector2>();
            Debug.Log("Move input start event, move value shows " + moveValue[0] + "," + moveValue[1]);

            if (!gridMovementMode)
            {
                // No forward input shouldn't immediately stop us; instead, we only track positive vs. negative and let deceleration slow us.
                if (moveValue[1] != 0)
                {
                    forwardDirectionFactor = moveValue[1];
                    Debug.Log($"{context.action} started as move input is pressed");
                    moving = true;
                }
            }
            else
            {
                // move player forward
                float x_translation = gridSpeed * moveValue[0];
                float y_translation = gridSpeed * moveValue[1];
                float z_translation = 0.0F;
                Vector3 translation_vector = new Vector3(x_translation, y_translation, z_translation);
                Debug.Log($"Moving by {translation_vector}");
                transform.Translate(translation_vector);
            }
        }

        void OnMoveInputEndEvent(CallbackContext context)
        {
            /*
            Vector2 moveValue = context.action.ReadValue<Vector2>();
            Debug.Log("Move input end event, move value shows " + moveValue[0] + "," + moveValue[1]);
            if (moveValue[1] == 0)
            {
                Debug.Log($"{context.action} stopped as move input is released");
                moving = false;
            }
            */
        }

        /**
         * Modifies acceleration and speed over time. Goal is linear acceleration and exponential speed.
         */
        void OnAccelerationPeriodEvent()
        {
            if (!gridMovementMode)
            {
                // check to see if move action was maintained through an accel period
                if (previouslyMoving == moving)
                {
                    if (moving)
                    {
                        acceleration += accelerationRate;
                        speed += acceleration;
                    }
                    else
                    {
                        if (speed > 0.0F)
                        {
                            deceleration += decelerationRate;
                            speed -= deceleration;
                        }
                        else
                        {
                            acceleration = 0.0F;
                            deceleration = 0.0F;
                        }
                    }
                }
                speed = Mathf.Clamp(speed, 0.0F, 100.0F);
                Debug.Log("Accel event! Speed: " + speed);
                previouslyMoving = moving;
            }
        }
    }
}