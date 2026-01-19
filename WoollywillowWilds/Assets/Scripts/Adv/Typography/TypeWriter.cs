using System;
using TMPro;
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
        /// The TMP_Text textview we wish to write into.
        /// </summary>
        public TMP_Text targetTextView;
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
        private UnityEvent writeEvent = new UnityEvent();
        /// <summary>
        /// The current index position we should write to storyview on the next OnWriteEvent().
        /// </summary>
        private int textPosition = 0;

        /// <summary>
        /// Converts TextToTypeWrite into a character array and launches coroutines on
        /// a delay based on typeWriterDelayMilliseconds.
        /// </summary>
        void TypeWrite()
        {
            // todo: timing to simulate characters being written at configurable delay, like on a typewriter.
            //   These have to process in sequence, so we can't fire and forget all at once; instead, each coroutine completion
            //   should check to see if the array is empty and launch a new writer coroutine if not
            //   until all characters have been written. Else, could maybe do a loop through the character
            //   array with awaits/yields?
            float derivedWriteDelay = typeWriterDelayMilliseconds;
            if (randomDelay)
            {
                System.Random rnd = new System.Random();
                derivedWriteDelay += rnd.Next(-randomDelayRangeMilliseconds, randomDelayRangeMilliseconds);
                derivedWriteDelay = Math.Clamp(derivedWriteDelay, 0, float.MaxValue);
            }
            writeEvent.AddListener(OnWriteEvent);
            ClockworkTasks clocks = gameObject.GetComponent<ClockworkTasks>();
            clocks.LaunchClock("TypeWriterEvent", writeEvent, 0, true, derivedWriteDelay);

            // todo: play sound effect like the Camelot Shining series alongside timed type events?
        }

        void OnWriteEvent()
        {
            int derivedChunkSize = characterChunkSize;
            if (randomChunkSize)
            {
                System.Random rnd = new System.Random();
                derivedChunkSize += rnd.Next(-randomChunkSizeRange, randomChunkSizeRange);
                derivedChunkSize = Math.Clamp(derivedChunkSize, 1, int.MaxValue);
            }
            // todo: check that we have derivedChunkSize characters left. If not, send the last of what we have.
            targetTextView.text += TextToTypeWrite.Substring(textPosition, derivedChunkSize);
            // todo: if we've sent the last of the TextToTypeWrite corpus, StopCoroutine(ClockworkTasks.clockroutineMap["TypeWriterEvent"]) to cancel the TypeWriterEvent tagged coroutine.
        }
    }
}
