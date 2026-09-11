using UnityEngine;
using System.Collections;
using MoodMap = System.Collections.Generic.Dictionary<WildsAdv.Mood, System.Collections.Generic.List<UnityEngine.AudioClip>>;

namespace WildsAdv
{
    /// <summary>
    /// Interrupts an sfx stream with a single AudioClip selected from InterruptAudioClips.
    /// todo: would be nice to be able to set a duration and have the populated array of clips run
    /// only as long as that duration (if duration is less than the combined clips' length) or
    /// keeps selecting from the array until duration is met (if duration exceeds combined clips' length).
    /// todo: would also be nice to be able to play through an entire array of AudioClips in a single interrupt
    /// event if desired, to avoid having to populated lots of AudioClip arrays across multiple interrupt SOs
    /// in the Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "PrefabClipInterrupt.asset", menuName = "SoundAndEffects/PrefabClipInterruptSO")]
    public class SfxInterruptPrefabClipSO : SfxInterruptSO
    {
        /// <summary>
        /// True to select randomly from within the pool of InterruptAudioClips, false
        /// to traverse the array sequentially per interrupt event.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField]
        public bool RandomizeClip { get; set; }
        /// <summary>
        /// The array of AudioClips to play through as the interruption sfx.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField]
        public List<AudioClip> InterruptAudioClips { get; set; }
        /// <summary>
        /// Tracks where we are in the InterruptAudioClips array if RandomizeClip is false.
        /// </summary>
        private int iterativeSfxIndex = 0;
        override public IEnumerator Interrupt(IInterruptableSfx _interruptableSfx)
        {
            AudioClip interruptTrack;
            Mood mood = interruptableSfx.QueryMood();
            MoodMap moodMap = interruptableSfx.QueryMoodMap();
            if (moodMap.ContainsKey(mood))
            {
                List<AudioClip> moodTracks = moodMap[mood];
                if (RandomizeClip)
                {
                    Random rnd = new Random();
                    int clipIndex = rnd.Next(0, moodTracks.Count - 1);
                    interruptTrack = moodTracks[clipIndex];
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
                    interruptTrack = moodTracks[iterativeSfxIndex];
                }
            }
            else
            {
                if (RandomizeClip)
                {
                    Random rnd = new Random();
                    int clipIndex = rnd.Next(0, AudioClips.Count);
                    interruptTrack = AudioClips[clipIndex];
                    Debug.Log("Playing " + interruptTrack.name + " for " + interruptTrack.length + ", from index " + clipIndex);
                }
                else
                {
                    if (iterativeSfxIndex < AudioClips.Count - 1)
                    {
                        iterativeSfxIndex++;
                    }
                    else
                    {
                        iterativeSfxIndex = 0;
                    }
                    interruptTrack = AudioClips[iterativeSfxIndex];
                    Debug.Log("Playing " + interruptTrack.name + " for " + interruptTrack.length + ", from index " + iterativeSfxIndex);
                }
            }
            // cache the main stream track so we can resume it after the interrupt completes.
            AudioSource player = interruptableSfx.QueryPlayer();
            if (interruptTrack != null)
            {
                player.resource = interruptTrack;
            }
            player.Play();
            yield return new WaitUntil(() => player.time >= interruptTrack.length);
            player.Stop();
        }
    }
}
