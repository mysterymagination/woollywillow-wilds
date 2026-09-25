using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WildsAdv
{
    /// <summary>
    /// A sentence containing associated mood and AudioClip. The property setters are private because we overload Equals() and therefore GetHashCode(), and the hashcode is computed
    /// based on the property values; it can be dangerous to have runtime-mutable values used to derive a hashcode since that could lead to the hashcode changing while the
    /// object is stored in a hashtable in-memory.
    /// </summary>
    [Serializable]
    public class VibeTrack
    {
        public VibeTrack(Mood mood, AudioClip clip)
        {
            TrackClip = clip;
            TrackMood = mood;
        }
        [field: SerializeField]
        public AudioClip TrackClip { get; private set; }
        [field: SerializeField]
        public Mood TrackMood { get; private set; } = Mood.Neutral;

        override public string ToString()
        {
            return "{\n  \"clip name\": \"" + TrackClip.name + "\",\n  \"mood\": \"" + TrackMood + "\"\n}";
        }

        public override bool Equals(object obj)
        {
            VibeTrack otherVibe = (VibeTrack)obj;
            return otherVibe.TrackMood == TrackMood
            && otherVibe.TrackClip.Equals(TrackClip);
        }
        public override int GetHashCode()
        {
            return JsonUtility.ToJson(this).GetHashCode();
        }
    }
    /// <summary>
    /// An array of AudioClips in which each clip can be associated with a mood; these will help inform
    /// which voice sfx track/segment should play for text of different moods.
    /// </summary>
    [Serializable]
    public class MoodTraxSO : ScriptableObject
    {
        /**
         * An array of AudioClip to Mood associations.
         */
        public List<VibeTrack> Vibes = new List<VibeTrack>();

        /// <summary>
        /// Dumps the raw text of the Vibes array into a basic string.
        /// </summary>
        /// <returns>string containing the text data only.</returns>
        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (VibeTrack vibe in Vibes)
            {
                sb.Append(vibe);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}