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
        /// EDIT: with a single AudioSource field I've found we run into a bottleneck of calling Play -> Play and then nothing playing at all. You have to Pause/Stop in between, which sounds bad. Instead, I'm going to try out
        /// adding/removing AudioSource Components as needed at runtime and allowed each to play out in its own Coroutine.
        /// </summary>
        public AudioClip typingSfx;
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
        /// The current time point in seconds at which the next sfx AudioSource should play.
        /// This should be tracked as sfxTimePivot modulo sfxTimeRandomRange plus whatever jump
        /// or continuous tracking we may want.
        /// </summary>
        private float sfxTimePoint = 0.0F;
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
        /// The function called by our typewrite Coroutine, representing one or more characters being stamped
        /// by the hammer onto paper. The delay simulates human typing speed.
        /// </summary>
        private IEnumerator writeFunction;

        /// <summary>
        /// Resets the stateful fields of TypeWriter so it can be re-used at runtime. Does not modify public configurable fields.
        /// </summary>
        public void ResetState()
        {
            textPosition = 0;
            sfxTimePoint = 0.0F;
        }

        /// <summary>
        /// Converts TextToTypeWrite into a character array and launches coroutines on
        /// a delay based on typeWriterDelayMilliseconds.
        /// </summary>
        public void TypeWrite()
        {
            writeFunction = AsyncWrite(0.0F);
            Debug.Log("WriteThing IEnumerator about to start is " + writeFunction + ", and a second call preparing that function gives us IEnumerator " + AsyncWrite(1.0F));

            // we want to interrupt any old Coroutine hosting this code, so stop any currently running before starting the new guy.
            StopCoroutine(writeFunction);
            StartCoroutine(writeFunction);
        }

        IEnumerator AsyncWrite(float initDelayMs)
        {
            yield return new WaitForSeconds(initDelayMs / 1000.0F);
            int initialDelayChunkSize = OnWriteEvent();
            while (textPosition < TextToTypeWrite.Length && initialDelayChunkSize > 0)
            {
                // calculate our writeevent period
                float loopPeriodMs = typeWriterDelayMilliseconds;
                if (randomDelay)
                {
                    System.Random rnd = new System.Random();
                    loopPeriodMs += rnd.Next(0, randomDelayRangeMilliseconds);
                    loopPeriodMs = Math.Clamp(loopPeriodMs, 0, float.MaxValue);
                }
                Debug.Log("Write event period ms is " + loopPeriodMs);
                Debug.Log("About to delay for " + loopPeriodMs + "ms before keystrokin");
                yield return new WaitForSeconds(loopPeriodMs / 1000.0F);
                int loopDelayChunkSize = OnWriteEvent();

                // The time it takes for the key-hammer sound to occur should not
                // affect the typing cadence, only vice-versa.
                StartCoroutine(AsyncSfx(loopDelayChunkSize, loopPeriodMs));
            }
        }

        /// <summary>
        /// Executes write behavior, writing a chunk of characters to the text sink.
        /// </summary>
        /// <returns>The number of characters written in this chunk.</returns>
        public int OnWriteEvent()
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
                return derivedChunkSize;
            }
            else
            {
                Debug.LogError("Target textview TMP_Text Component is unset");
                return 0;
            }
        }

        IEnumerator AsyncSfx(int charactersWritten, float typingCadence)
        {
            if (typingSfx)
            {


                /*
                float sfxDurationMs = typingCadence - charactersWritten * keyHammerStrikeTimeMilliseconds;
                sfxDurationMs = Math.Clamp(sfxDurationMs, keyHammerStrikeTimeMilliseconds, keyHammerStrikeTimeMilliseconds + typingCadence);
                yield return new WaitForSeconds(sfxDurationMs / 1000.0F);
                */
                /*
                yield return new WaitForSeconds(typingSfx.clip.length);
                */

                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.resource = typingSfx;
                source.Play();
                yield return new WaitForSeconds(typingSfx.length);
            }
        }

        protected void PauseSfx()
        {
            if (typingSfx)
            {
                /*
                if (typingSfx.isPlaying)
                {
                    
                    if (typingSfx.time >= typingSfx.clip.length)
                    {
                        sfxTimePoint = 0.0F;
                    }
                    else
                    {
                        sfxTimePoint = typingSfx.time;
                        Debug.Log("SFX timepoint saved as " + sfxTimePoint);
                    }
                    
                typingSfx.Pause();
                }
                */
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
                /*
                typingSfx.Stop();
                */
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
