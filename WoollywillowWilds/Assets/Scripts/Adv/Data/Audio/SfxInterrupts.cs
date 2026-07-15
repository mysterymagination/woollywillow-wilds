using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MoodMap = System.Collections.Generic.Dictionary<WildsAdv.Mood, System.Collections.Generic.List<UnityEngine.AudioClip>>;

namespace WildsAdv
{
    /// <summary>
    /// SFX interrupt base class with common properties.
    /// </summary>
    [Serializable]
    public abstract class SfxInterrupt
    {
        public SfxInterrupt(float timeOffset)
        {
            TimeOffset = timeOffset;
        }
        [field: SerializeField]
        public float TimeOffset { get; private set; }
        /// <summary>
        /// Interrupts the SFX currently being played by the input player for the input sentence.
        /// The manner of this interruption depends on the particular <see cref="SfxInterrupt"/> subclass. 
        /// </summary>
        /// <param name="interruptableSfx">The <see cref="IInterruptableSfx"/> playing the main SFX stream which we wish to interrupt. It implements query functions that inform our interrupt details.</param>
        /// <returns>An <see cref="IEnumerator"/> handle for Coroutine resume after suspend.</returns>
        public abstract IEnumerator Interrupt(IInterruptableSfx interruptableSfx);
    }
    /// <summary>
    /// An SFX interrupt that plays an AudioClip. The property setters are private because we overload Equals() and therefore GetHashCode(), and the hashcode is computed
    /// based on the property values; it can be dangerous to have runtime-mutable values used to derive a hashcode since that could lead to the hashcode changing while the
    /// object is stored in a hashtable in-memory.
    /// </summary>
    [Serializable]
    public class AudioClipInterrupt : SfxInterrupt
    {
        public AudioClipInterrupt(float timeOffset, AudioClip interruptClip, bool randomizeClip, List<AudioClip> audioClips) : base(timeOffset)
        {
            InterruptClip = interruptClip;
            RandomizeClip = randomizeClip;
            AudioClips = audioClips;
        }
        [field: SerializeField]
        public AudioClip InterruptClip { get; private set; }
        [field: SerializeField]
        public bool RandomizeClip { get; private set; }
        [field: SerializeField]
        public List<AudioClip> AudioClips { get; private set; }
        private int iterativeSfxIndex = 0;
        override public string ToString()
        {
            return "{\n  \"clip name\": \"" + InterruptClip.name + "\",\n  \"time offset\": \"" + TimeOffset + "\"\n  \"randomize clip\": \"" + RandomizeClip + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            AudioClipInterrupt otherInterrupt = (AudioClipInterrupt)obj;
            return otherInterrupt.TimeOffset == TimeOffset
            && otherInterrupt.InterruptClip.Equals(InterruptClip)
            && otherInterrupt.RandomizeClip == RandomizeClip
            && otherInterrupt.AudioClips == AudioClips;
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }

        override public IEnumerator Interrupt(IInterruptableSfx interruptableSfx)
        {
            AudioClip interruptTrack;
            Mood mood = interruptableSfx.QueryMood();
            MoodMap moodMap = interruptableSfx.QueryMoodMap();
            if (moodMap.ContainsKey(mood))
            {
                List<AudioClip> moodTracks = moodMap[mood];
                if (RandomizeClip)
                {
                    System.Random rnd = new System.Random();
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
                    System.Random rnd = new System.Random();
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
            // cache the steady-state track so we can resume it after the interrupt completes.
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
    /// <summary>
    /// An SFX interrupt that simply pauses the SFX, causing silence for a time. The property setters are private because we overload Equals() and therefore GetHashCode(), and the hashcode is computed
    /// based on the property values; it can be dangerous to have runtime-mutable values used to derive a hashcode since that could lead to the hashcode changing while the
    /// object is stored in a hashtable in-memory.
    /// </summary>
    [Serializable]
    public class LacunaInterrupt : SfxInterrupt
    {
        public LacunaInterrupt(float timeOffset, float duration) : base(timeOffset)
        {
            Duration = duration;
        }
        public LacunaInterrupt(float timeOffset, float durationMin, float durationMax) : base(timeOffset)
        {
            Duration = UnityEngine.Random.Range(durationMin, durationMax);
        }
        [field: SerializeField]
        public float Duration { get; private set; }
        override public string ToString()
        {
            return "{\n  \"lacuna duration\": \"" + Duration + "\",\n  \"time offset\": \"" + TimeOffset + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            LacunaInterrupt otherInterrupt = (LacunaInterrupt)obj;
            return otherInterrupt.TimeOffset == TimeOffset
            && otherInterrupt.Duration == Duration;
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }

        override public IEnumerator Interrupt(IInterruptableSfx _interruptableSfx)
        {
            yield return new WaitForSecondsRealtime(Duration);
        }
    }
    /// <summary>
    /// An SFX interrupt that changes the entire SfxMode temporarily. This allows us to e.g. switch algorithmically between prefab chirps and algorithmically clipped chirps
    /// for great variance and the best of both worlds.
    /// </summary>
    [Serializable]
    public class FunctionalInterrupt : SfxInterrupt
    {
        public FunctionalInterrupt(float timeOffset, float duration, SfxMode mode) : base(timeOffset)
        {
            Duration = duration;
            Mode = mode;
        }
        [field: SerializeField]
        public float Duration { get; private set; }
        [field: SerializeField]
        public SfxMode Mode { get; private set; }
        override public string ToString()
        {
            return "{\n  \"functional interrupt duration\": \"" + Duration + "\",\n  \"time offset\": \"" + TimeOffset + "\"\n \"sfxMode algo\": \"" + Mode + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            FunctionalInterrupt otherInterrupt = (FunctionalInterrupt)obj;
            return otherInterrupt.TimeOffset == TimeOffset
            && otherInterrupt.Duration == Duration
            && otherInterrupt.Mode == Mode;
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }
        public override IEnumerator Interrupt(IInterruptableSfx interruptableSfx)
        {
            // TODO: many ofthe SfxMode async functions drop into forever loops that rely on the host coroutine being stopped from an external vantage; we don't have that here at the moment.
            yield return interruptableSfx.OnFunctionalInterrupt(Mode);
        }
    }

    /// <summary>
    /// An array of SFX interrupts of various concrete types to be injected into a steady-state SFX stream.
    /// </summary>
    [Serializable]
    public class SfxInterrupts
    {
        /**
         * An ordered array of TreasureSentences, each associated with relevant Mood and presented in the
         * desired sequence for the final text rendering.
         */
        public List<SfxInterrupt> Interrupts = new List<SfxInterrupt>();

        /// <summary>
        /// Dumps the raw text of the Interrupts array into a basic string.
        /// </summary>
        /// <returns>string containing the text data only.</returns>
        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SfxInterrupt interrupt in Interrupts)
            {
                sb.Append(interrupt);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}