using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;
namespace WildsAdv
{
    public class ClockworkTasks : MonoBehaviour
    {
        [Serializable]
        public class TimedEvent
        {
            /**
             * The UnityEvent to run.
             */
            public UnityEvent unityEvent;
            /** 
             * Delay before the event initially runs, in seconds.
             */
            public float delay;
            /** 
             * Period at which the event should repeat after the initial run iff loop is true, in seconds.
             */
            public float period;
            /** 
             * True to run the event periodically every period seconds after the initial run, false to run only once.
             */
            public bool loop;
        }

        public class TimedTask
        {
            public TimedTask() { }
            public TimedTask(IEnumerator delayHandle)
            {
                initDelayHandle = delayHandle;
            }
            public TimedTask(IEnumerator delayHandle, IEnumerator periodHandle)
            {
                initDelayHandle = delayHandle;
                periodicHandle = periodHandle;
            }
            public IEnumerator initDelayHandle;
            public IEnumerator periodicHandle;
        }

        [SerializeField] public Dictionary<string, TimedTask> timedTaskMap = new Dictionary<string, TimedTask>();

        [SerializeField] public Dictionary<string, Coroutine> clockroutineMap = new Dictionary<string, Coroutine>();

        /// <summary>
        /// Launches a Coroutine wrapping a function call that will invoke the input unityEvent after the given delay and then again at the given period iff loop is true.
        /// This Coroutine will remain active in the clockroutineMap under the input tag key and may be reused with StartCoroutine() or cancelled with StopCoroutine().
        /// </summary>
        /// <param name="eventKey">string handle we want to key our new Coroutine in clockroutineMap; if there is already a Coroutine at this key, it will be stopped to ensure we don't leak potentially active Coroutines.</param>
        /// <param name="unityEvent">The event to be invoked when the Coroutine runs its code.</param>
        /// <param name="delay">Initial delay before first invocation, in seconds.</param>
        /// <param name="loop">True if the event should be invoked repeatedly forever (until the Coroutine is explicitly stopped), false if it should only invoke once.</param>
        /// <param name="period">The period at which the event will be invoked subsequent to the first invocation iff loop is true.</param>
        public void LaunchClock(string eventKey, UnityEvent unityEvent, float delay, bool loop = false, float period = 0.0f)
        {
            StopAllCoroutines();
            if (clockroutineMap.ContainsKey(eventKey))
            {
                StopCoroutine(clockroutineMap[eventKey]);
            }
            Coroutine clockRoutine = StartCoroutine(InvokeDelayed(unityEvent, delay, loop, period));
            clockroutineMap[eventKey] = clockRoutine;
        }

        /// <summary>
        /// Calls StopCoroutine() on the Coroutine mapped to the input eventKey.
        /// </summary>
        /// <param name="eventKey">The eventKey used for this event in LaunchClock()</param>
        /// <returns>true if a Coroutine was found and stopped, false if clockroutineMap did not contain the input eventKey.</returns>
        public bool StopClock(string eventKey)
        {
            if (clockroutineMap.ContainsKey(eventKey))
            {
                StopCoroutine(clockroutineMap[eventKey]);
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator InvokeDelayed(UnityEvent unityEvent, float delay, bool loop = false, float period = 0.0f)
        {
            yield return new WaitForSeconds(delay);
            //Debug.Log("Invoking delayed event after delay of " + delay + " seconds.");
            unityEvent.Invoke();
            if (loop)
            {
                while (true)
                {
                    //Debug.Log("Inside loop.");
                    yield return new WaitForSeconds(period);
                    //Debug.Log("Invoking looped event at period " + period + " seconds.");
                    unityEvent.Invoke();
                }
            }
            else
            {
                Debug.Log("No looping desired, coroutine work is finished for now.");
            }
        }

        public void LaunchClock_RequeueApproach(string tag, TimedEvent timedEvent)
        {
            IEnumerator delayTaskHandle = InvokeDelayed_RequeueApproach(tag, timedEvent.unityEvent, timedEvent.delay, timedEvent.loop);
            TimedTask task = new TimedTask(delayTaskHandle);
            if (timedEvent.loop)
            {
                task.periodicHandle = InvokeDelayed_RequeueApproach(tag, timedEvent.unityEvent, timedEvent.period, timedEvent.loop);
            }
            timedTaskMap.Add(tag, task);
            StartCoroutine(delayTaskHandle);
        }

        private IEnumerator InvokeDelayed_RequeueApproach(string tag, UnityEvent unityEvent, float delay, bool loop)
        {
            yield return new WaitForSeconds(delay);
            unityEvent.Invoke();
            if (loop)
            {
                TimedTask task = new TimedTask();
                if (timedTaskMap.TryGetValue(tag, out task))
                {
                    StartCoroutine(task.periodicHandle);
                }
            }
        }

        [CustomEditor(typeof(ClockworkTasks))]
        public class ClockworkTasksInspector : Editor
        {
            private SerializedProperty EventDelayPairs;
            private ReorderableList list;

            private ClockworkTasks _clockworkTasksScript;

            private void OnEnable()
            {
                _clockworkTasksScript = (ClockworkTasks)target;

                EventDelayPairs = serializedObject.FindProperty("EventDelayPairs");

                list = new ReorderableList(serializedObject, EventDelayPairs)
                {
                    draggable = true,
                    displayAdd = true,
                    displayRemove = true,
                    drawHeaderCallback = rect =>
                    {
                        EditorGUI.LabelField(rect, "DelayedEvents");
                    },
                    drawElementCallback = (rect, index, sel, act) =>
                    {
                        var element = EventDelayPairs.GetArrayElementAtIndex(index);

                        var unityEvent = element.FindPropertyRelative("unityEvent");
                        var delay = element.FindPropertyRelative("Delay");


                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), delay);

                        rect.y += EditorGUIUtility.singleLineHeight;

                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(unityEvent)), unityEvent);


                    },
                    elementHeightCallback = index =>
                    {
                        var element = EventDelayPairs.GetArrayElementAtIndex(index);

                        var unityEvent = element.FindPropertyRelative("unityEvent");

                        var height = EditorGUI.GetPropertyHeight(unityEvent) + EditorGUIUtility.singleLineHeight;

                        return height;
                    }
                };
            }

            public override void OnInspectorGUI()
            {
                DrawScriptField();

                serializedObject.Update();

                list.DoLayoutList();

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawScriptField()
            {
                // Disable editing
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(_clockworkTasksScript), typeof(ClockworkTasks), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
            }
        }
    }
}