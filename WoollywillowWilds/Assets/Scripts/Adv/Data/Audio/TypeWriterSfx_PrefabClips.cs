
namespace WildsAdv
{
    /// <summary>
    /// Asynchronously loops through a mood-mapped and/or iterative set of prefabricated AudioClips.
    /// Each clip is played in a Coroutine that yield returns a WaitForSecondsRealtime(X seconds) where
    /// X is the length of the AudioClip; this way we can fire off clips and let them play to completion
    /// before moving on to the next one, until an external entity stops us.  
    /// </summary>
    public class TypeWriterSfx_PrefabClips : MonoBehaviour, ITypeWriterSfx, IInterruptableSfx
    {
        /// <summary>
        /// An array of AudioClips to play through in the event that we don't have one that matches the current mood.
        /// </summary>
        public AudioClip[] defaultSfxArray;
        /// <summary>
        /// Whether or not we should set the sfx clip index to a random value around the current one within sfxClipIndexRange after a full stop breath.
        /// </summary>
        public bool randomSfxClipIndex = false;
        /// <summary>
        /// AudioClip to mood associations; these will be massaged into an in-memory Dictionary in Setup().
        /// </summary>
        public MoodTrax sfxVibes;
        /// <summary>
        /// Tracks the current index into the defaultSfxArray.
        /// </summary>
        private int sfxIndex = 0;
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
            if (sfxBlipIndex >= typingSfxBlipArray.Length)
            {
                sfxBlipIndex = 0;
            }
            AudioClip typingSfx = typingSfxBlipArray[sfxBlipIndex];
            sfxBlipIndex++;
            if (typingSfx)
            {
                if (sfxBlipIndex >= typingSfxBlipArray.Length)
                {
                    sfxBlipIndex = 0;
                }
                singularSfx.resource = typingSfxBlipArray[sfxBlipIndex];
                singularSfx.Play();
                sfxBlipIndex++;
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

    public IEnumerator OnFunctionalInterrupt(SfxMode mode, float duration)
        {
            IEnumerator interruptFunction = null;
            switch (mode)
            {
                case SfxMode.ChirpSentenceAlgoClipped:
                    interruptFunction = AsyncSfx_ChirpSentenceAlgoClipped(currentTrack);
                    break;
                case SfxMode.ChirpSentenceAlgoVariance:
                    interruptFunction = AsyncSfx_ChirpSentenceAlgoVariance(currentMood);
                    break;
                case SfxMode.VoicedSentenceArray:
                    interruptFunction = AsyncSfx_VoicedSentence(currentMood);
                    break;
            }

            // todo: what happens if the 'parent' sfx coroutine we're currently running from, presumably AsyncSfx_ChirpSentencePrefabVariance(),
            //  gets stopped before we have the chance to stop this 'child' sfx coroutine? Is there a callback Coroutines get when stopped?
            //  EDIT: looks like nothing built in; you can sort of hack it yourself, but that would involve storing the IEnumerator handle we get
            //   here somewhere higher up? Perhaps maintain a list of SFX stuff to kill when a sentence ends?
            if (interruptFunction != null)
            {
                StartCoroutine(interruptFunction);
                yield return new WaitForSeconds(duration);
                StopCoroutine(interruptFunction);
            }
        }

        public AudioSource QueryPlayer()
        {
            return player;
        }

        public Mood QueryMood()
        {
            return mood;
        }

        public Dictionary<Mood, List<AudioClip>> QueryMoodMap()
        {
            return moodTracksMap;
        }
    }