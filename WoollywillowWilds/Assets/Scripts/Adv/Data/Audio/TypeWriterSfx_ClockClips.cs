
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace WildsAdv
{
    /// <summary>
    /// Asynchronously loops through an array of AudioClips and associated durations.
    /// Each clip is played in a Coroutine that yield returns a WaitForSecondsRealtime(X seconds) where
    /// X is the associated desired duration; if the duration exceeds the AudioClip length, we loop it until
    /// duration expires. Otherwise, no individual track loops, but the entire array loops back around to index 0
    /// when it completes. 
    /// </summary>
    public class TypeWriterSfx_ClockClips : MonoBehaviour, ITypeWriterSfx
    {
        /// <summary>
        /// An array of AudioClip to duration associations.
        /// </summary>
        public TimeTraxSO trackArray;
        [Range(0.0F, 1.0F)]
        public float Volume { get; set; } = 0.5F;
        private AudioSource player;
        private IEnumerator sfxFunction;

        public void Setup(ScriptableObject sfxData)
        {
            TypeWriterSfx_PrefabClipsDataSO clipsSfxData = (TypeWriterSfx_PrefabClipsDataSO)sfxData;
            if (clipsSfxData)
            {
                clipsSfxData.Populate(this);
            }

            if (moodTracksMap.Count == 0)
            {
                if (sfxVibes != null && sfxVibes.Vibes.Count > 0)
                {
                    foreach (VibeTrack vibe in sfxVibes.Vibes)
                    {
                        if (!moodTracksMap.ContainsKey(vibe.TrackMood))
                        {
                            moodTracksMap.Add(vibe.TrackMood, new List<AudioClip>());
                        }
                        moodTracksMap[vibe.TrackMood].Add(vibe.TrackClip);
                    }
                }
            }

            player = gameObject.AddComponent<AudioSource>();
            player.loop = true;
            player.volume = Volume;
        }
        public void Teardown()
        {
            moodTracksMap.Clear();
            Destroy(player);
        }
        public void Play()
        {
            sfxFunction = AsyncSfx_MainStream(CurrentMood);
            StartCoroutine(sfxFunction);
            if (Interrupts.Length > 0)
            {
                interruptFunction = AsyncSfx_Interrupt();
                StartCoroutine(interruptFunction);
            }
        }
        public void Pause()
        {
            player.Pause();
        }

        public void Stop()
        {
            player.Stop();
            StopCoroutine(sfxFunction);
            // trill support
            if (TrillingClipFraction < 1.0F)
            {
                StartCoroutine(AsyncSfx_TrillCompletion());
            }

            if (currentSfxInterrupt != null)
            {
                Debug.Log("Shutting down interrupt man from host.");
                currentSfxInterrupt.Stop();
                currentSfxInterrupt.Teardown();
                Component sfxComponent = (Component)currentSfxInterrupt;
                if (sfxComponent)
                {
                    UnityEngine.Object.Destroy(sfxComponent);
                }
                currentSfxInterrupt = null;
            }

            // stop interrupt coroutine if relevant.
            if (interruptFunction != null)
            {
                StopCoroutine(interruptFunction);
            }
        }


        IEnumerator AsyncSfx_MainStream(Mood mood)
        {
            // todo: since the main stream coroutine is independent of the interrupt stream coroutine we'll need a way to effectively pause the main stream itself,
            //  I guess just via a condition inside the loop that skips its logic, else the main stream could start playing itself again during an insterruption.

            int iterativeSfxIndex = 0;
            // loop forever, depending on the calling control flow to stop the host coroutine.
            while (true)
            {
                player.Stop();
                if (moodTracksMap.ContainsKey(mood))
                {
                    List<AudioClip> moodTracks = moodTracksMap[mood];
                    if (randomSfxClipIndex)
                    {
                        System.Random rnd = new System.Random();
                        int clipIndex = rnd.Next(0, moodTracks.Count - 1);
                        currentTrack = moodTracks[clipIndex];
                    }
                    else
                    {
                        if (iterativeSfxIndex < moodTracks.Count - 1)
                        {
                            iterativeSfxIndex++;
                        }
                        else
                        {
                            iterativeSfxIndex = 0;
                        }
                        currentTrack = moodTracks[iterativeSfxIndex];
                    }
                }
                else
                {
                    if (randomSfxClipIndex)
                    {
                        System.Random rnd = new System.Random();
                        int clipIndex = rnd.Next(0, defaultSfxArray.Length);
                        currentTrack = defaultSfxArray[clipIndex];
                        Debug.Log("Playing " + currentTrack.name + " for " + currentTrack.length + ", from index " + clipIndex);
                    }
                    else
                    {
                        if (iterativeSfxIndex < defaultSfxArray.Length - 1)
                        {
                            iterativeSfxIndex++;
                        }
                        else
                        {
                            iterativeSfxIndex = 0;
                        }
                        currentTrack = defaultSfxArray[iterativeSfxIndex];
                        Debug.Log("Playing " + currentTrack.name + " for " + currentTrack.length + ", from index " + iterativeSfxIndex);
                    }
                }
                if (currentTrack != null)
                {
                    player.resource = currentTrack;
                }
                player.loop = true;
                player.Play();

                float clipFraction = Math.Clamp(TrillingClipFraction, 0.0F, 1.0F);
                if (ClipFractionRandomization)
                {
                    float range = clipFraction / 2.0F;
                    clipFraction = UnityEngine.Random.Range(clipFraction - range, clipFraction + range);
                }
                yield return new WaitUntil(() => player.time >= currentTrack.length * clipFraction);
            }
        }
    }
}
