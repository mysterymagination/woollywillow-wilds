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
        /// Sound effect played during write events. By default, this will
        /// begin at the beginning of the track and play unmodified until it
        /// is instructed to stop; the write events' delay will be correlated
        /// with the sfx playing, so normally the entire track will not get to play.
        /// Thus, the parameters below allow for a degree of randomness in where
        /// and how audio is played from the sfx source.
        /// If sfxPivotPoint is set, each fresh play will occur at or around that time in the track. If sfxContinuous is set, each fresh play will pick up where the track left off last (modulo sfxTimeRandomRange) and sfxPivotPoint will be ignored.
        /// </summary>
        public AudioSource typingSfx;
        /// <summary>
        /// Percentage amount +/- to change the default sfx volume.
        /// </summary>
        public float sfxVolumeRandomRange = 0.0F;
        /// <summary>
        /// Percentage amount +/- to change the default sfx pitch.
        /// </summary>
        public float sfxPitchRandomRange = 0.0F;
        /// <summary>
        /// The time point in seconds in the track around which any sfxTimeRandomRange
        /// should move the starting point for the next play. If there is no
        /// random range, this value will just be the constant starting time point
        /// whenever the track is played.
        /// </summary>
        public float sfxTimePivot = 0.0F;
        /// <summary>
        /// Amount of seconds +/- the sfxTimePivot point where the track will start playing next.
        /// </summary>
        public float sfxTimeRandomRange = 0.0F;
        /// <summary>
        /// If true, the sfx time point will gradually progress per write event. It will still be subject to sfxTimeRandomRange, but sfxTimePivot will be ignored.
        /// </summary>
        public bool sfxContinuous = true;

        /// <summary>
        /// The current index position we should write to storyview on the next OnWriteEvent().
        /// </summary>
        private int textPosition = 0;
        /// <summary>
        /// The function called by our typewrite Coroutine.
        /// </summary>
        private IEnumerator writeFunction;
        //private float sfxTimePoint = 0.0F;

        /// <summary>
        /// Resets the stateful fields of TypeWriter so it can be re-used at runtime. Does not modify public configurable fields.
        /// </summary>
        public void ResetState()
        {
            textPosition = 0;
            //sfxTimePoint = 0.0F;
            if (typingSfx)
            {
                typingSfx.time = 0.0F;
            }
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

            writeFunction = AsyncWrite(0.0F, derivedWriteDelay);
            Debug.Log("WriteThing IEnumerator about to start is " + writeFunction);

            // we want to interrupt any old Coroutine hosting this code, so stop any currently running before starting the new guy.
            StopCoroutine(writeFunction);
            StartCoroutine(writeFunction);
        }

        IEnumerator AsyncWrite(float initDelay, float loopPeriod)
        {
            // start and stop sfx around write events so that we play for the delay.
            //ContinuePlayingSfx();
            yield return new WaitForSeconds(initDelay);
            bool successWrite = OnWriteEvent();
            //PauseSfx();
            while (textPosition < TextToTypeWrite.Length && successWrite)
            {
                //ContinuePlayingSfx();
                PauseSfx();
                yield return new WaitForSeconds(loopPeriod);
                successWrite = OnWriteEvent();
                //PauseSfx();
                // so this approach with sound on write and pause at delay follows the voice replaces typewriter keys+hammer
                // paradigm, but the write event itself essentially takes 0 time so... may need to add a 'render' (both sound and graphics, why not) delay
                // inside OnWriteEvent() or here?
                ContinuePlayingSfx();
            }
            PauseSfx();
        }

        public bool OnWriteEvent()
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
            if (targetTextViewComponent)
            {
                targetTextViewComponent.text += storyChunk;
                Debug.Log("Full story text is now: {" + targetTextViewComponent.text + "}.");
                // update textPosition to the next unwritten segment.
                textPosition += derivedChunkSize;
                return true;
            }
            else
            {
                Debug.LogError("Target textview TMP_Text Component is unset");
                return false;
            }
        }

        protected void ContinuePlayingSfx()
        {
            if (typingSfx)
            {
                PauseSfx();
                float randoVal = UnityEngine.Random.Range(-sfxTimeRandomRange, sfxTimeRandomRange);
                float derivedTimePoint = typingSfx.time + randoVal;//sfxTimePoint + randoVal;
                Debug.Log("Unclamped randomized derived sfx timepoint timepoint " + derivedTimePoint + ", from saved timepoint " + typingSfx.time + " plus random generated float " + randoVal);
                // this will sometimes produce illegal seek values, presumably at the clip.length extrema.
                // we can't really do anything about that AFAIK since using an upper bound less than
                // clip.length causes loop mode to never reset time, and we get stuck at some note
                // close to the end past the artificial max forever with or without loop.
                derivedTimePoint = Math.Clamp(derivedTimePoint, 0, typingSfx.clip.length);
                Debug.Log("Setting sfx timepoint from saved timepoint " + typingSfx.time + " to rando derived timepoint " + derivedTimePoint);
                typingSfx.time = derivedTimePoint;
                typingSfx.Play();
            }
        }

        protected void PauseSfx()
        {
            if (typingSfx)
            {
                if (typingSfx.isPlaying)
                {
                    /*
                    if (typingSfx.time >= typingSfx.clip.length)
                    {
                        sfxTimePoint = 0.0F;
                    }
                    else
                    {
                        sfxTimePoint = typingSfx.time;
                        Debug.Log("SFX timepoint saved as " + sfxTimePoint);
                    }
                    */
                    typingSfx.Pause();
                }
            }
        }

        public bool Shutdown(bool clear)
        {
            bool succesfulShutdown = true;
            if (writeFunction != null)
            {
                StopCoroutine(writeFunction);
            }
            else
            {
                Debug.LogError("Shutdown; writeFunction is null so we cannot stop the write Coroutine.");
                succesfulShutdown = false;
            }
            if (targetTextViewComponent)
            {
                if (clear)
                {
                    targetTextViewComponent.text = "";
                }
            }
            else
            {
                Debug.LogError("Shutdown; target textview is unset, so we cannot clear its text.");
                succesfulShutdown = false;
            }
            if (typingSfx)
            {
                typingSfx.Stop();
            }
            ResetState();
            return succesfulShutdown;
        }

        public void OnCanvasClose()
        {
            Shutdown(true);
        }

        public void OnFastForward()
        {
            if (Shutdown(false))
            {
                if (targetTextViewComponent)
                {
                    targetTextViewComponent.text = TextToTypeWrite;
                }
                else
                {
                    Debug.LogError("OnFastForward; target textview is unset, so we cannot skip ahead to full story text");
                }
            }
            else
            {
                Debug.LogError("OnFastForward; shutting down the typewriter failed, so we cannot skip ahead to full story text.");
            }
        }
    }
}
