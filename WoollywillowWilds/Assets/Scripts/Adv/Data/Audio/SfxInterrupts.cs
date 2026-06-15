using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    }
    /// <summary>
    /// An SFX interrupt that simply pauses the SFX, causing silence for a time. The property setters are private because we overload Equals() and therefore GetHashCode(), and the hashcode is computed
    /// based on the property values; it can be dangerous to have runtime-mutable values used to derive a hashcode since that could lead to the hashcode changing while the
    /// object is stored in a hashtable in-memory.
    /// </summary>
    [Serializable]
    public class LacunaInterrupt : SfxInterrupt
    {
        public AudioClipInterrupt(float timeOffset, float duration) : base(timeOffset)
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