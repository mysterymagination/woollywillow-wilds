using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Events;

namespace WildsAdv
{
    /// <summary>
    /// Component that writes text to a TMP_Text textview at configurable delay to simulate a typewriter.
    /// </summary>
    public class TypeWriter : MonoBehaviour
    {
        /// <summary>
        /// The TMP_Text textview Component we wish to write into.
        /// </summary>
        public TMP_Text targetTextViewComponent;
        /// <summary>
        /// The ClockworkTasks Component that will manage timing for us.
        /// </summary>
        public ClockworkTasks clockComponent;
        /// <summary>
        /// The delay time in between typewritten characters being written to the text view, in milliseconds.
        /// If randomDelay is set, the delay will be a random number generated in randomDelayRangeMilliseconds +/- this value.
        /// </summary>
        public ulong typeWriterDelayMilliseconds;
        /// <summary>
        /// Set to add randomness to the typewriter delay to simulate real life tempo changes.
        /// </summary>
        public bool randomDelay = false;
        /// <summary>
        /// An amount of milliseconds more or less than typeWriterDelayMilliseconds (clamped at 0 floor) to be used as the delay value if randomDelay is set.
        /// </summary>
        public int randomDelayRangeMilliseconds = 0;
        /// <summary>
        /// The number of characters to pull from the source TextToTypeWrite per OnWriteEvent(). Modified by randomChunkSize and randomChunkSizeRange.
        /// </summary>
        public int characterChunkSize = 1;
        /// <summary>
        /// Set to add randomness to the typewriter character set chunk size to simulate real life tempo changes.
        /// </summary>
        public bool randomChunkSize = false;
        /// <summary>
        /// An amount of characters more or less than characterChunkSize (clamped at 0 floor) to pull from the source TextToTypeWrite per OnWriteEvent().
        /// </summary>
        public int randomChunkSizeRange = 0;
        /// <summary>
        /// Property that stores the text string to be written with typewriter effects.
        /// </summary>
        public string TextToTypeWrite { get; set; }

        /// <summary>
        /// Timed event whose invocation will call the necessary handlers to write the next chunk of characters to the storyview.
        /// </summary>
        public UnityEvent writeEvent;
        /// <summary>
        /// The current index position we should write to storyview on the next OnWriteEvent().
        /// </summary>
        private int textPosition = 0;
        /// <summary>
        /// The key at which our typewriter Coroutine will be stored in ClockworkTasks.clockroutineMap.
        /// </summary>
        private string eventKey = "TypeWriterEvent";
        private string debugStory = "";

        /// <summary>
        /// Resets the stateful fields of TypeWriter so it can be re-used at runtime. Does not modify public configurable fields.
        /// </summary>
        public void ResetState()
        {
            textPosition = 0;
            debugStory = "";
            // todo: stop any coroutine at the eventKey.
        }

        /// <summary>
        /// Converts TextToTypeWrite into a character array and launches coroutines on
        /// a delay based on typeWriterDelayMilliseconds.
        /// </summary>
        public void TypeWrite()
        {
            // calculate our writeevent period
            float derivedWriteDelay = typeWriterDelayMilliseconds;
            if (randomDelay)
            {
                System.Random rnd = new System.Random();
                derivedWriteDelay += rnd.Next(-randomDelayRangeMilliseconds, randomDelayRangeMilliseconds);
                derivedWriteDelay = Math.Clamp(derivedWriteDelay, 0, float.MaxValue);
            }
            // ClockworkTasks API expects time in seconds.
            derivedWriteDelay /= 1000.0F;
            Debug.Log("Write event period is " + derivedWriteDelay);

            // todo: this API only allows for constant periodicity; what I'd really like is to have each writeevent come down at a slightly randomized period.
            //   The Coroutine = StartCoroutine(IEnumerator) API is kinda weird and rigid anyway; I'd prefer a nice async/await inna loop that's controlled client-side.
            //   That's doable with the current API via passing `false` for looping and using the derivedWriteDelay as the initial (and only) delay argument, but I
            //   want to explore the other available C#/Unity structured concurrency biz.
            // todo: this hard dependency of one Component on a sibling in order to have any use feels like an antipattern; better to have this guy's job be to take the
            //   text input and figure out how chunking it over time should work, and OnWriteEvent() would call up to a provided event which would be implemented back in RoomItem
            //   since he knows about the storytextview already... that said, RoomItem is just another Component and therefore not in any better position than this guy for
            //   finding required siblings e.g. ClockworkTasks. I dunno if we can escape external dependencies cleanly, but it might be better to have interface fields rather than
            //   specific Component implementations here so that users of this Component can supply interface impls however they please and this Component remains decoupled from
            //   any specific siblings.
            if (clockComponent)
            {
                clockComponent.LaunchClock(eventKey, writeEvent, 0, true, derivedWriteDelay);
            }
            else
            {
                Debug.LogError("ClockworkTasks component is missing, so we cannot schedule timed writes.");
            }


            /* this works
            StopAllCoroutines();
            IEnumerator functor = WriteThing(0.0F, derivedWriteDelay);
            Debug.Log("WriteThing IEnumerator about to start is " + functor);
            StartCoroutine(functor);
            */

            // todo: play sound effect like the Camelot Shining series alongside timed type events?
        }

        IEnumerator WriteThing(float initDelay, float loopPeriod)
        {
            yield return new WaitForSeconds(initDelay);
            OnWriteEvent();
            //uint count = 0;
            //while (count < 25)
            while (textPosition < TextToTypeWrite.Length)
            {
                yield return new WaitForSeconds(loopPeriod);
                OnWriteEvent();
                //count++;
            }
        }

        public void OnWriteEvent()
        {
            int derivedChunkSize = characterChunkSize;
            if (randomChunkSize)
            {
                System.Random rnd = new System.Random();
                derivedChunkSize += rnd.Next(-randomChunkSizeRange, randomChunkSizeRange);
                derivedChunkSize = Math.Clamp(derivedChunkSize, 1, int.MaxValue);
            }
            // check that we have derivedChunkSize characters left. If not, send the last of what we have.
            if (textPosition + derivedChunkSize > TextToTypeWrite.Length)
            {
                derivedChunkSize = TextToTypeWrite.Length - textPosition;
            }
            Debug.Log("Story chunk size: " + derivedChunkSize);
            string storyChunk = TextToTypeWrite.Substring(textPosition, derivedChunkSize);
            Debug.Log("Writing story chunk: " + storyChunk);
            targetTextViewComponent.text += storyChunk;
            debugStory += storyChunk;
            Debug.Log("Full story text is now: {" + targetTextViewComponent.text + "}.");
            Debug.Log("Debug story text is now: {" + debugStory + "}.");
            // update textPosition to the next unwritten segment.
            textPosition += derivedChunkSize;
            // if we've sent the last of the TextToTypeWrite corpus, StopCoroutine(ClockworkTasks.clockroutineMap[eventKey]) to cancel the TypeWriterEvent tagged coroutine.
            if (textPosition >= TextToTypeWrite.Length)
            {
                Debug.Log("Finished writing story chunks; shutting down coroutine loop. Our final debug story says: {" + debugStory + "}.");
                // todo: need a way to have closing the item detail canvas also short circuit this typewriter coroutine... most direct approach specifically for the non-transient FrameCanvas
                //   would be to have the ItemCanvasUnloader Component look for a TypeWriter in parent GameObject and then call a shutdown function. Better would be if we could broadcast
                //   and event from ItemCanvasUnloader (and the CanvasUnloader function of ItemCanvasLoader, for transient canvasi) that says "we're goin' down!" and have Components respond as necessary. 
                if (clockComponent)
                {
                    bool stoppedAnything = clockComponent.StopClock(eventKey);
                    Debug.Log(stoppedAnything ? "Stopped the write event" : "Failed to stop the write event");
                }
                else
                {
                    Debug.LogError("ClockworkTasks component is missing, so we cannot stop the typewriting clock.");
                }
            }
        }
    }
}
