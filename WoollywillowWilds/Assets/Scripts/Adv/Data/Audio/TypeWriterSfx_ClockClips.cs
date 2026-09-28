
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
        public TimeTraxSO trackTimes;
        [Range(0.0F, 1.0F)]
        public float Volume { get; set; } = 0.5F;
        private AudioSource player;
        private IEnumerator sfxFunction;

        public void Setup(ScriptableObject sfxData)
        {
            player = gameObject.AddComponent<AudioSource>();
            player.loop = false;
            player.volume = Volume;
        }
        public void Teardown()
        {
            Destroy(player);
        }
        public void Play()
        {
            sfxFunction = AsyncSfx_MainStream();
            StartCoroutine(sfxFunction);
        }
        public void Pause()
        {
            player.Pause();
        }

        public void Stop()
        {
            player.Stop();
            StopCoroutine(sfxFunction);
        }


        IEnumerator AsyncSfx_MainStream()
        {
            int iterativeSfxIndex = 0;
            // loop forever, depending on the calling control flow to stop the host coroutine.
            while (true)
            {
                player.Stop();
                TimeTrack currentTrack = trackTimes.Tracks[iterativeSfxIndex];
                Debug.Log("Playing " + currentTrack.TrackClip.name + " for " + currentTrack.TrackDuration + ", from index " + iterativeSfxIndex);
                if (currentTrack != null)
                {
                    player.resource = currentTrack.TrackClip;
                }
                player.Play();
                if (currentTrack.TrackClip.length < currentTrack.TrackDuration)
                {
                    player.loop = true;
                }
                yield return new WaitForSeconds(currentTrack.TrackDuration);
                player.loop = false;
                if (iterativeSfxIndex < trackTimes.Tracks.Count - 1)
                {
                    iterativeSfxIndex++;
                }
                else
                {
                    iterativeSfxIndex = 0;
                }
            }
        }
    }
}
