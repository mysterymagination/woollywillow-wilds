using TMPro;
using UnityEngine;

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
        /// An amount of milliseconds more or less than typeWriterDelayMilliseconds to be used as the delay value if randomDelay is set.
        /// </summary>
        public ulong randomDelayRangeMilliseconds = 0;
        /// <summary>
        /// Property that stores the text string to be written with typewriter effects.
        /// </summary>
        public string TextToTypeWrite { get; set; }

        // Update is called once per frame
        void Update()
        {
            // todo: timing to simulate characters being written at configurable delay, like on a typewriter.
            // todo: play sound effect like the Camelot Shining series alongside timed type events?
        }
    }
}
