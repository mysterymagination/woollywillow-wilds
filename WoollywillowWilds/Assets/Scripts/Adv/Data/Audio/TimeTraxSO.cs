using System.Text;
using UnityEngine;

namespace WildsAdv
{
    /// <summary>
    /// A sentence containing associated AudioClip and duration.
    /// </summary>
    [Serializable]
    public class TimeTrack
    {
        public TimeTrack(float duration, AudioClip clip)
        {
            TrackClip = clip;
            TrackDuration = duration;
        }
        [field: SerializeField]
        public AudioClip TrackClip { get; set; }
        /// <summary>
        /// Duration the track should play, in seconds.
        /// </summary>
        [Min(0F)]
        public float Duration { get; set; } = 1.0F;

        override public string ToString()
        {
            return "{\n  \"clip name\": \"" + TrackClip.name + "\",\n  \"duration\": \"" + TrackDuration + "\"\n}";
        }
    }
    /// <summary>
    /// An array of AudioClips in which each clip can be associated with a duration for which
    /// the track should play.
    /// </summary>
    [Serializable]
    public class TimeTraxSO : ScriptableObject
    {
        /**
         * An array of AudioClip to duration associations.
         */
        public List<TimeTrack> Tracks = new List<TimeTrack>();

        /// <summary>
        /// Dumps the raw text of the Tracks array into a basic string.
        /// </summary>
        /// <returns>string containing the text data only.</returns>
        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TimeTrack track in Tracks)
            {
                sb.Append(track);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}