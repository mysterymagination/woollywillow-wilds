using System;
using System.Collections;
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
        [field: SerializeField]
        public TreasureText TextToTypeWrite { get; set; }
        /// <summary>
        /// Delay after each sentence to separate sentences visually and by SFX.
        /// </summary>
        [field: SerializeField]
        public float BreathDelayMs { get; set; } = 1000.0F;
        /// <summary>
        /// The current index position we should write to storyview on the next OnWriteEvent().
        /// </summary>
        private int textPosition = 0;
        /// <summary>
        /// The function called by our typewrite Coroutine, representing one or more characters being stamped
        /// by the hammer onto paper. The delay simulates human typing speed.
        /// </summary>
        private IEnumerator writeFunction;
        public TypeWriterSfx_Blips blipsSfx;
        public TypeWriterSfx_KeyHammer keyHammerSfx;
        public TypeWriterSfx_PrefabClips prefabsSfx;
        private AudioClip currentTrack;

        /// <summary>
        /// Resets the stateful fields of TypeWriter so it can be re-used at runtime. Does not modify public configurable fields.
        /// </summary>
        public void ResetState()
        {
            textPosition = 0;
        }

        /// <summary>
        /// Converts TextToTypeWrite into a character array and launches coroutines on
        /// a delay based on typeWriterDelayMilliseconds.
        /// </summary>
        public void TypeWrite()
        {
            writeFunction = AsyncWrite();

            blipsSfx?.Setup();
            keyHammerSfx?.Setup();
            prefabsSfx?.Setup();

            // we want to interrupt any old Coroutine hosting this code, so stop any currently running before starting the new guy.
            StopCoroutine(writeFunction);
            StartCoroutine(writeFunction);
        }

        /// <summary>
        /// Yields WaitForSeconds() for the configured period while looping over character chunks to type out for each sentence in the TextToTypeWrite TreasureText.
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncWrite()
        {
            foreach (TreasureSentence currentTreasureSentence in TextToTypeWrite.Contents)
            {
                // todo: look ahead for fullstop and mod volume/pitch etc. based on punctuation e.g. louder for `!`
                textPosition = 0;

                if (prefabsSfx)
                {
                    prefabsSfx.CurrentMood = currentTreasureSentence.SentenceMood;
                    prefabsSfx.Play();
                }


                while (textPosition < currentTreasureSentence.SentenceText.Length)
                {
                    // calculate our writeevent period
                    float loopPeriodMs = typeWriterDelayMilliseconds;
                    if (randomDelay)
                    {
                        System.Random blipRnd = new System.Random();
                        loopPeriodMs += blipRnd.Next(0, randomDelayRangeMilliseconds);
                        loopPeriodMs = Math.Clamp(loopPeriodMs, 0, float.MaxValue);
                    }
                    Debug.Log("Write event period ms is " + loopPeriodMs);
                    Debug.Log("About to delay for " + loopPeriodMs + "ms before keystrokin");

                    if (blipsSfx)
                    {
                        blipsSfx.Play();
                    }
                    if (keyHammerSfx)
                    {
                        keyHammerSfx.Play();
                    }

                    yield return new WaitForSeconds(loopPeriodMs / 1000.0F);

                    // write and update pos.
                    string storyChunkWritten = OnWriteEvent(textPosition, currentTreasureSentence);
                    textPosition += storyChunkWritten.Length;

                    if (blipsSfx)
                    {
                        blipsSfx.Pause();
                    }
                } // end sentence
                // single whitespace after fullstop.
                if (targetTextViewComponent)
                {
                    targetTextViewComponent.text += " ";
                }

                if (prefabsSfx)
                {
                    prefabsSfx.Pause();
                }

                // take a breath after sentence completion.
                yield return new WaitForSeconds(BreathDelayMs / 1000.0F);
            } // end text
        }

        /// <summary>
        /// Executes write behavior, writing a chunk of characters to the text sink.
        /// </summary>
        /// <param name="currentTextPosition">The current 0-based index into the current TreasureSentence's text property.</param>
        /// <param name="currentSentence">The current TreasureSentence we're typing out.</param>
        /// <returns>The characters written in this chunk.</returns>
        public string OnWriteEvent(int currentTextPosition, TreasureSentence currentSentence)
        {
            int derivedChunkSize = characterChunkSize;
            if (randomChunkSize)
            {
                System.Random rnd = new System.Random();
                derivedChunkSize += rnd.Next(-randomChunkSizeRange, randomChunkSizeRange);
                derivedChunkSize = Math.Clamp(derivedChunkSize, 1, int.MaxValue);
            }
            // check that we have derivedChunkSize characters left. If not, send the last of what we have.
            if (currentTextPosition + derivedChunkSize > currentSentence.SentenceText.Length)
            {
                derivedChunkSize = currentSentence.SentenceText.Length - currentTextPosition;
            }
            Debug.Log("Story chunk size: " + derivedChunkSize);
            string storyChunk = currentSentence.SentenceText.Substring(currentTextPosition, derivedChunkSize);
            Debug.Log("Writing story chunk: " + storyChunk);
            if (targetTextViewComponent)
            {
                targetTextViewComponent.text += storyChunk;
                return storyChunk;
            }
            else
            {
                Debug.LogError("Target textview TMP_Text Component is unset");
                return "";
            }
        }

        protected void PauseSfx()
        {
            /*
            if (typingSfx)
            {
                
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
                
            }
            */
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
            /*
            if (typingSfx)
            {
                
                typingSfx.Stop();
                
            }
            */
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
                    targetTextViewComponent.text = TextToTypeWrite.ExtractText();
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
