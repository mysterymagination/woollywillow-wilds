using System;
using System.Collections;
using System.Text;
using UnityEngine;

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
        public abstract IEnumerator Interrupt(float duration);
    }
    /// <summary>
    /// An SFX interrupt that plays an AudioClip. The property setters are private because we overload Equals() and therefore GetHashCode(), and the hashcode is computed
    /// based on the property values; it can be dangerous to have runtime-mutable values used to derive a hashcode since that could lead to the hashcode changing while the
    /// object is stored in a hashtable in-memory.
    /// </summary>
    [Serializable]
    public class AudioClipInterrupt : SfxInterrupt
    {
        public AudioClipInterrupt(float timeOffset, AudioClip clip) : base(timeOffset)
        {
            TrackClip = clip;
        }
        [field: SerializeField]
        public AudioClip TrackClip { get; private set; }
        override public string ToString()
        {
            return "{\n  \"clip name\": \"" + TrackClip.name + "\",\n  \"time offset\": \"" + TimeOffset + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            AudioClipInterrupt otherInterrupt = (AudioClipInterrupt)obj;
            return otherInterrupt.TimeOffset == TimeOffset
            && otherInterrupt.TrackClip.Equals(TrackClip);
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }

        public IEnumerator Interrupt(float duration)
        {

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