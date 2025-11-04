using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;

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

    public void LaunchClock(string tag, UnityEvent unityEvent, float delay, bool loop = false, float period = 0.0f)
    {
        Coroutine clockRoutine = StartCoroutine(InvokeDelayed(unityEvent, delay, loop, period));
        clockroutineMap.Add(tag, clockRoutine);
    }

    private IEnumerator InvokeDelayed(UnityEvent unityEvent, float delay, bool loop = false, float period = 0.0f)
    {
        yield return new WaitForSeconds(delay);
        unityEvent.Invoke();
        if (loop)
        {
            while (true)
            {
                yield return new WaitForSeconds(period);
                unityEvent.Invoke();
            }
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
