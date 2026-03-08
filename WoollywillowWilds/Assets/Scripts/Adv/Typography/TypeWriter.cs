using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

namespace WildsAdv
{
    public enum SfxMode
    {
        /**
         * This mode plays sfx per write event, mimicking the sound of a typewriter key press and hammer stamp on paper.
         * In theory this mode might make the most technically accurate bond between the rendering of the text and
         * the accompanying sounds, but in practice it's difficult to make this sound 'good' for values of good that
         * include sounding like indistinct speech.
         */
        KeyHammer,
        /// <summary>
        /// A mix of keyhammer and voice tone, we use a singular AudioSource to plau through a large array of short AudioClips.
        /// </summary>
        BlipArray,
        /**
         * This mode steps through the typingSfxSegmentMap, optionally jumping to named segments, in accordance
         * with completed sentences in the text. Any fullstop character will stop the current sfx and there will
         * be a pause before the next sentence picks up either where we left off or at an optionally randomized new
         * track position and/or track. Optional emote tags in the text can also direct which track should play
         * for a particular sentence.
         */
        VoicedSentence,
    }
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
        public SfxMode sfxMode = SfxMode.VoicedSentence;
        /// <summary>
        /// Named sound effects played during write events. By default, this will
        /// begin at the beginning of the track and play looping unmodified until it
        /// is instructed to stop.
        /// </summary>
        public Dictionary<string, AudioClip> typingSfxSegmentMap;
        /// <summary>
        /// Short sound effects played 1:1 with write events. By default, this will
        /// begin at the 0 index clip and proceed through until the end at which point it will
        /// wrap around. Each clip will play to completion without looping, simulating a typewriter
        /// key-hammer stroke or a single voice tone syllable.
        /// </summary>
        public AudioClip[] typingSfxBlipArray;
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
        /// Amount of milliseconds to pause both typing and sfx at fullstops.
        /// </summary>
        public float breathDelayMs = 1000.0F;
        /// <summary>
        /// Amount of clip elements +/- the current index that will be selected to play next after a full stop breath.
        /// </summary>
        public int sfxClipIndexRange = 0;
        /// <summary>
        /// Whether or not we should set the sfx clip index to a random value around the current one within sfxClipIndexRange after a full stop breath.
        /// </summary>
        public bool randomSfxClipIndex = false;

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
        /// The function called by our sfx Coroutine, representing the cadence of a natural voice alongside the typing.
        /// todo: do we need to overlap sfx at any point? If so, we'll need to map these to the relevant sfx and maybe cancel the looping on everyone
        /// who isn't the primary sfx but then let them play out to completion? An event delegate might be the cleanest way to do that, and may avoid
        /// the need to enmap everyone.
        /// </summary>
        private IEnumerator sfxFunction;
        /// <summary>
        /// The current time point in seconds at which the next sfx AudioSource should play.
        /// This should be tracked as sfxTimePivot modulo sfxTimeRandomRange plus whatever jump
        /// or continuous tracking we may want.
        /// </summary>
        private float sfxTimePoint = 0.0F;
        /// <summary>
        /// Tracks the current index into the typingSfxBlipArray.
        /// </summary>
        private int sfxBlipIndex = 0;
        /// <summary>
        /// Used for the VoicedSentence sfx mode, where we only have one sfx playing at any given time. We'll pause, seek, and play this same AudioSource as necessary in that mode.
        /// </summary>
        private AudioSource singularSfx;
        /// <summary>
        /// Mapping of text mood associations to an array of suitable sfx clips.
        /// </summary>
        public Dictionary<Mood, AudioClip[]> voiceSfxSegmentMap;

        /// <summary>
        /// Resets the stateful fields of TypeWriter so it can be re-used at runtime. Does not modify public configurable fields.
        /// </summary>
        public void ResetState()
        {
            textPosition = 0;
            sfxTimePoint = 0.0F;
            sfxBlipIndex = 0;
            Destroy(singularSfx);
        }

        /// <summary>
        /// Converts TextToTypeWrite into a character array and launches coroutines on
        /// a delay based on typeWriterDelayMilliseconds.
        /// </summary>
        public void TypeWrite()
        {
            writeFunction = AsyncWrite(0.0F);
            if (sfxMode == SfxMode.VoicedSentence || sfxMode == SfxMode.BlipArray)
            {
                singularSfx = gameObject.AddComponent<AudioSource>();
            }
            sfxFunction = AsyncSfx(0, 0);
            Debug.Log("WriteThing IEnumerator about to start is " + writeFunction + ", and a second call preparing that function gives us IEnumerator " + AsyncWrite(1.0F));

            // we want to interrupt any old Coroutine hosting this code, so stop any currently running before starting the new guy.
            StopCoroutine(writeFunction);
            StartCoroutine(writeFunction);
        }

        IEnumerator AsyncWrite(float initDelayMs)
        {
            yield return new WaitForSeconds(initDelayMs / 1000.0F);
            string initStoryChunkWritten = OnWriteEvent();
            while (textPosition < TextToTypeWrite.Length && initStoryChunkWritten.Length > 0)
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

                //todo: we want to support the synchronous blip array in BlipArray and also the asynchronous coroutine blip array for keyhammer.
                if (sfxMode == SfxMode.BlipArray)
                {
                    if (sfxBlipIndex >= typingSfxBlipArray.Length)
                    {
                        sfxBlipIndex = 0;
                    }
                    singularSfx.resource = typingSfxBlipArray[sfxBlipIndex];
                    singularSfx.Play();
                    sfxBlipIndex++;
                }
                yield return new WaitForSeconds(loopPeriodMs / 1000.0F);
                string storyChunkWritten = OnWriteEvent();
                singularSfx.Pause();
                string fullStopPattern = "[.!?:;…]";
                // todo: mod volume/pitch etc. based on punctuation e.g. louder for `!`

                // The time it takes for the key-hammer sound to occur should not
                // affect the typing cadence, only vice-versa.

                if (Regex.Matches(storyChunkWritten, fullStopPattern).Count > 0)
                {
                    if (randomSfxClipIndex)
                    {
                        int cachedBlipIndex = sfxBlipIndex;
                        System.Random rnd = new System.Random();
                        int indexModifier = rnd.Next(-sfxClipIndexRange, sfxClipIndexRange);
                        sfxBlipIndex += indexModifier;
                        sfxBlipIndex = Math.Clamp(sfxBlipIndex, 0, typingSfxBlipArray.Length - 1);
                        Debug.Log("Randomizing blip index from " + cachedBlipIndex + " to " + sfxBlipIndex + " based on index mod " + indexModifier);
                    }
                    yield return new WaitForSeconds(breathDelayMs / 1000.0F);
                }
            }
        }

        /// <summary>
        /// Executes write behavior, writing a chunk of characters to the text sink.
        /// </summary>
        /// <returns>The characters written in this chunk.</returns>
        public string OnWriteEvent()
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
                return storyChunk;
            }
            else
            {
                Debug.LogError("Target textview TMP_Text Component is unset");
                return "";
            }
        }

        IEnumerator AsyncSfx(int charactersWritten, float typingCadence)
        {
            if (sfxBlipIndex >= typingSfxBlipArray.Length)
            {
                sfxBlipIndex = 0;
            }
            AudioClip typingSfx = typingSfxBlipArray[sfxBlipIndex];
            sfxBlipIndex++;
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

                // remove the host AudioSource Component at the bottom of the Coroutine functor.
                Destroy(source);
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
