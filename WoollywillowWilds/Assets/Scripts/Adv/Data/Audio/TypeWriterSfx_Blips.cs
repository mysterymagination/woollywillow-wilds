
namespace WildsAdv
{
    /// <summary>
    /// A mix of keyhammer and voice tone, we use a singular AudioSource to play through a large array of short AudioClips
    /// while a sentence types itself out. Each new write event cuts off the previous sfx and replaces it with a new one.
    /// </summary>
    public class TypeWriterSfx_Blips : MonoBehaviour, ITypeWriterSfx
    {
        /// <summary>
        /// Short sound effects played 1:1 with write events. By default, this will
        /// begin at the 0 index clip and proceed through until the end at which point it will
        /// wrap around. Each clip will play to completion without looping, simulating a typewriter
        /// key-hammer stroke or a single voice tone syllable.
        /// </summary>
        public AudioClip[] typingSfxBlipArray;
        /// <summary>
        /// Whether or not we should set the sfx clip index to a random value around the current one within sfxClipIndexRange after a full stop breath.
        /// </summary>
        public bool randomSfxClipIndex = false;
        /// <summary>
        /// AudioClip to mood associations; these will be massaged into an in-memory Dictionary in Setup().
        /// </summary>
        public MoodTrax blipSfxVibes;
        /// <summary>
        /// Tracks the current index into the typingSfxBlipArray.
        /// </summary>
        private int sfxBlipIndex = 0;
        private AudioSource player;
        private Mood mood;
        private Dictionary<Mood, List<AudioClip>> moodTracksMap;
        public void Setup()
        {
            if (moodTracksMap.Count == 0)
            {
                if (blipSfxVibes != null && blipSfxVibes.Vibes.Count > 0)
                {
                    foreach (VibeTrack vibe in blipSfxVibes.Vibes)
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
            player.volume = sfxVolume;
        }
        public void Teardown()
        {
            sfxBlipIndex = 0;
            moodTracksMap.Clear();
            Destroy(player);
        }
        public void Play()
        {
            AudioClip currentTrack = null;
            if (moodTracksMap.ContainsKey(mood))
            {
                List<AudioClip> moodTracks = moodTracksMap[mood];
                if (randomSfxClipIndex)
                {
                    Random rnd = new Random();
                    int clipIndex = rnd.Next(0, moodTracks.Count - 1);
                    currentTrack = moodTracks[clipIndex];
                }
                else
                {
                    if (sfxBlipIndex >= typingSfxBlipArray.Length)
                    {
                        sfxBlipIndex = 0;
                    }
                    currentTrack = moodTracks[sfxBlipIndex];
                    sfxBlipIndex++;
                }
            }
            else
            {
                if (randomSfxClipIndex)
                {
                    Random rnd = new Random();
                    int clipIndex = rnd.Next(0, moodTracks.Count - 1);
                    currentTrack = typingSfxBlipArray[clipIndex];
                }
                else
                {
                    if (sfxBlipIndex >= typingSfxBlipArray.Length)
                    {
                        sfxBlipIndex = 0;
                    }
                    currentTrack = typingSfxBlipArray[sfxBlipIndex];
                    sfxBlipIndex++;
                }
            }
            if (currentTrack)
            {
                singularSfx.resource = currentTrack;
                singularSfx.Play();
            }
        }
        public void Pause()
        {
            player.Pause();
            if (randomSfxClipIndex)
            {
                int cachedBlipIndex = sfxBlipIndex;
                System.Random blipRnd = new System.Random();
                int indexModifier = blipRnd.Next(-sfxClipIndexRange, sfxClipIndexRange);
                sfxBlipIndex += indexModifier;
                sfxBlipIndex = Math.Clamp(sfxBlipIndex, 0, typingSfxBlipArray.Length - 1);
                Debug.Log("Randomizing blip index from " + cachedBlipIndex + " to " + sfxBlipIndex + " based on index mod " + indexModifier);
            }
        }
    }
}
