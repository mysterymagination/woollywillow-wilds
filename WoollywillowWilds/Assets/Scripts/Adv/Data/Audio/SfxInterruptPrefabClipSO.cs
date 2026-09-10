using UnityEngine;
using System.Collections;

namespace WildsAdv
{
    [CreateAssetMenu(fileName = "PrefabClipInterrupt.asset", menuName = "SoundAndEffects/PrefabClipInterruptSO")]
    public class SfxInterruptPrefabClipSO : SfxInterruptSO
    {
        /// <summary>
        /// Total duration of the interrupt; if this is 0, the PrefabClips interrupt
        /// will run through the length of each interrupt clip by default. Otherwise,
        /// the interrupt stream will cut off after Duration has elapsed regardless of
        /// the interrupt clip progression.
        /// </summary>
        [Header("Audio Options")]
        [field: SerializeField] public float Duration { get; set; } = 0.0F;
        /// <summary>
        /// True to select randomly from within the pool of InterruptAudioClips, false
        /// to traverse the array sequentially.
        /// todo: to ensure we eventually stop the interruption, we'll need to track
        /// which clips have been used if Duration is unset; for random clips, this means
        /// tracking not just whether we've reached the end of the array by index but rather
        /// whether we've already played every clips without a known sequence. Simplest way
        /// would be to just count in both cases and quit when played count >= array size.
        /// A private setter for InterruptAudioClips might be nice for that case, depending on how the
        /// Inspector handles them, to ensure the pool of clips can't be modded once we're rollin'.
        /// To support deterministic interruption durations without making the designer calculate and set one
        /// ahead of time, it would be best to only allow each clip to play once; that means either rerolling when
        /// we hit a dup (and tracking clips played already) or removing clips played from a pool as we draw them,
        /// and then picking randomly from that runtime pool.
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
            yield return new WaitForSecondsRealtime(Duration);
        }
    }
}
